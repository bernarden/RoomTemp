import sqlite3

import TemperatureReading


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
        sql_parameters = (temp_reading.temperature, temp_reading.datetime.isoformat() + 'Z',
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
