#!/usr/bin/python

import getopt
import sys
from datetime import datetime, timedelta

from twisted.internet import task, reactor

from ApiClient import ApiClient
from depenencies.TSYS01 import TSYS01
from TemperatureReading import TemperatureReading
from Repository import Repository

TAKE_READING_EVERY_NUMBER_OF_SECONDS = 15

SYNC_MISSED_RECORDS_EVERY_NUMBER_OF_MINUTES = 20
MAX_NUMBER_OF_RECORDS_TO_SYNC_AT_ONCE = 50
ONLY_SYNC_MISSED_RECORDS_OLDER_THAN_NUMBER_OF_MINUTES = 10

CLEAN_UP_DB_EVERY_NUMBER_OF_HOURS = 24
CLEAN_UP_SYNCED_RECORDS_OLDER_THAN_NUMBER_OF_DAYS = 10


def main(argv):
    url, key, location_name, sensor_name = process_inputs(argv)
    repository = Repository()
    repository.setup_sqlite_database()

    sensor = TSYS01.TSYS01(0x76)

    api_client = ApiClient(url, key)
    location_id = api_client.get_location_id(location_name)
    sensor_id = api_client.get_sensor_id(sensor_name)

    clean_up_db_looping_call = task.LoopingCall(clean_up_db, repository)
    clean_up_db_looping_call.start(CLEAN_UP_DB_EVERY_NUMBER_OF_HOURS * 3600)

    sync_missed_records_looping_call = task.LoopingCall(sync_missed_records, api_client, repository)
    sync_missed_records_looping_call.start(SYNC_MISSED_RECORDS_EVERY_NUMBER_OF_MINUTES * 60)

    take_reading_looping_call = task.LoopingCall(take_reading, sensor, repository, api_client, location_id, sensor_id)
    take_reading_looping_call.start(TAKE_READING_EVERY_NUMBER_OF_SECONDS)

    reactor.run()


def take_reading(sensor, repository, api_client, location_id, sensor_id):
    try:
        temperature = sensor.read_temperature()
        temp_reading = TemperatureReading(temperature, datetime.utcnow(), location_id, sensor_id)
        print("New Reading:", temp_reading)
        reading_id = repository.insert_reading(temp_reading)
        result_code = api_client.post_temperature_reading(temp_reading)
        if result_code in (200, 201):
            repository.mark_reading_synced(reading_id)
    except Exception as e:
        print(e)


def clean_up_db(repository):
    try:
        older_than = datetime.utcnow() - timedelta(days=CLEAN_UP_SYNCED_RECORDS_OLDER_THAN_NUMBER_OF_DAYS)
        print(f"DB clean-up: Removing records older than {older_than}")
        records_deleted = repository.clean_up_old_synced_records(older_than)
        print(f"DB clean-up: Removed {records_deleted} records ")
    except Exception as e:
        print("DB clean-up:", e)


def sync_missed_records(api_client: ApiClient, repository: Repository):
    try:
        older_than = datetime.utcnow() - timedelta(minutes=ONLY_SYNC_MISSED_RECORDS_OLDER_THAN_NUMBER_OF_MINUTES)
        missed_readings = repository.get_unsynced_readings(MAX_NUMBER_OF_RECORDS_TO_SYNC_AT_ONCE, older_than)
        if len(missed_readings) == 0:
            print(f"Re-sync: Everything is already synced. Exiting.")
            return

        print(f"Re-sync: Syncing {len(missed_readings)} missed records.")
        result_code = api_client.post_db_temperature_readings(missed_readings)
        if result_code in (200, 201):
            repository.mark_readings_synced(list(map(lambda x: x.id, missed_readings)))
            print(f"Re-sync: Successfully synced {len(missed_readings)} records.")
            return

        print(f"Re-sync: Failed. Status code: {result_code}")
    except Exception as e:
        print("Re-sync: Failed.", e)


def process_inputs(argv):
    url = ''
    key = ''
    sensor_name = 'TSYS01'
    location_name = 'Home'

    input_help = 'RoomTemp.py -u <URL> -k <ApiKey> -l <LocationName> -s <SensorName>'
    try:
        opts, args = getopt.getopt(argv, "hu:k:l:s:", ["help", "url=", "key=", "location=", "sensor="])
    except getopt.GetoptError:
        print(input_help)
        sys.exit(2)
    for opt, arg in opts:
        if opt in ('-h', '--help'):
            print(input_help)
            sys.exit()
        elif opt in ("-u", "--url"):
            url = arg
        elif opt in ("-k", "--key"):
            key = arg
        elif opt in ("-l", "--location"):
            location_name = arg
        elif opt in ("-s", "--sensor"):
            sensor_name = arg

    if url == '':
        print("'Url' is a required argument. Please refer to --help for more information.")
        sys.exit(3)
    if key == '':
        print("'Key' is a required argument. Please refer to --help for more information.")
        sys.exit(3)

    return url, key, location_name, sensor_name


if __name__ == "__main__":
    main(sys.argv[1:])
