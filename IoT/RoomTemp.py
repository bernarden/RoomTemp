#!/usr/bin/python

import getopt
import sys
import datetime
from time import sleep

from ApiClient import ApiClient
from depenencies.TSYS01 import TSYS01
from TemperatureReading import TemperatureReading
from Repository import Repository


def main(argv):
    url, key, location_name, sensor_name = process_inputs(argv)
    repository = Repository()
    repository.setup_sqlite_database()

    sensor = TSYS01.TSYS01(0x76)

    client = ApiClient(url, key)
    location_id = client.get_location_id(location_name)
    sensor_id = client.get_sensor_id(sensor_name)

    while True:
        # noinspection PyBroadException
        try:
            temperature = sensor.read_temperature()
            temp_reading = TemperatureReading(temperature, datetime.datetime.utcnow(), location_id, sensor_id)
            print(temp_reading)

            reading_id = repository.insert_reading(temp_reading)
            result_code = client.post_temperature_reading(temp_reading)
            if result_code in (200, 201):
                repository.mark_reading_synced(reading_id)

        except Exception as e:
            print(e)
            pass
        finally:
            print("Sleeping for 15 seconds.")
            sleep(15)


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
