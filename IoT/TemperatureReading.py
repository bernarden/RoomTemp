from datetime import datetime
import dateutil.parser


class TemperatureReading(object):
    def __init__(self, temperature: float, taken_at: datetime, location_id: int, sensor_id: int):
        self.temperature = temperature
        self.taken_at = taken_at
        self.location_id = location_id
        self.sensor_id = sensor_id

    def __str__(self):
        return '{0.taken_at!s} {0.temperature!s} {0.location_id!s} {0.sensor_id!s}'.format(self)


class DbTemperatureReading(TemperatureReading):
    def __init__(self, id: int, temperature: float, taken_at: str, location_id: int, sensor_id: int, is_synced: int):
        taken_at_datetime = dateutil.parser.parse(taken_at)
        super().__init__(temperature, taken_at_datetime, location_id, sensor_id)
        self.id = id
        self.is_synced = is_synced != 0

    def __str__(self):
        return '{0.id!s} {0.is_synced!s} {1}'.format(self, super().__str__())
