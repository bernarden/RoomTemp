import datetime


class TemperatureReading(object):
    def __init__(self, temperature: float, datetime: datetime, location_id: int, sensor_id: int):
        self.temperature = temperature
        self.datetime = datetime
        self.location_id = location_id
        self.sensor_id = sensor_id

    def __str__(self):
        return '{0.datetime!s} {0.temperature!s} {0.location_id!s} {0.sensor_id!s}'.format(self)
