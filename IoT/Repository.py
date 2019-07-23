import sqlite3
from datetime import datetime
from typing import Sequence

from TemperatureReading import TemperatureReading, DbTemperatureReading


class Repository(object):
    def __init__(self, db_name='temperature.db'):
        self.connection = None
        self.db_name = db_name

    def setup_sqlite_database(self):
        self.connection = sqlite3.connect(self.db_name)
        cursor = self.connection.cursor()
        cursor.execute(
            '''CREATE TABLE IF NOT EXISTS TempReading ( 
            "Id" INTEGER NOT NULL CONSTRAINT "PK_TempReading" PRIMARY KEY AUTOINCREMENT,
            "Temperature" TEXT NOT NULL, 
            "TakenAt" TEXT NOT NULL, 
            "SensorId" INTEGER NOT NULL, 
            "LocationId" INTEGER NOT NULL,
            "IsSynced" INTEGER NOT NULL)''')
        self.connection.commit()

    def insert_reading(self, temp_reading: TemperatureReading):
        cursor = self.connection.cursor()
        sql_command = '''INSERT INTO TempReading (Temperature, TakenAt, LocationId, SensorId, IsSynced) 
                      VALUES (?,?,?,?,0)'''
        sql_parameters = (temp_reading.temperature, temp_reading.taken_at.isoformat() + 'Z',
                          temp_reading.location_id, temp_reading.sensor_id)
        cursor.execute(sql_command, sql_parameters)
        self.connection.commit()
        return cursor.lastrowid

    def mark_reading_synced(self, temp_reading_id: int):
        cursor = self.connection.cursor()
        sql_command = '''UPDATE TempReading SET IsSynced = 1 WHERE Id = ?'''
        sql_parameters = (temp_reading_id,)
        cursor.execute(sql_command, sql_parameters)
        self.connection.commit()

    def mark_readings_synced(self, temp_reading_ids: Sequence[int]):
        cursor = self.connection.cursor()
        sql_command = f'''UPDATE TempReading SET IsSynced = 1 WHERE Id in ({','.join(
            ['?'] * len(temp_reading_ids))})'''
        sql_parameters = temp_reading_ids
        cursor.execute(sql_command, sql_parameters)
        self.connection.commit()

    def clean_up_old_synced_records(self, older_than: datetime):
        cursor = self.connection.cursor()
        sql_command = '''DELETE FROM TempReading WHERE IsSynced = 1 AND TakenAt < ?'''
        sql_parameters = (older_than.isoformat() + 'Z',)
        cursor.execute(sql_command, sql_parameters)
        self.connection.commit()
        return cursor.rowcount

    def get_unsynced_readings(self, max_number_of_records: int = 20,
                              older_than: datetime = datetime.utcnow()) -> Sequence[DbTemperatureReading]:
        cursor = self.connection.cursor()
        sql_command = '''SELECT 
            "Id",
            "Temperature", 
            "TakenAt", 
            "LocationId",
            "SensorId", 
            "IsSynced"
            FROM TempReading 
            WHERE IsSynced = 0 AND TakenAt < ?
            ORDER BY TakenAt ASC 
            LIMIT ?'''
        sql_parameters = (older_than.isoformat() + 'Z', max_number_of_records)
        result = []
        for row in cursor.execute(sql_command, sql_parameters):
            result.append(DbTemperatureReading(row[0], row[1], row[2], row[3], row[4], row[5]))
        return result
