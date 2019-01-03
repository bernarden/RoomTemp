import requests
from typing import Sequence

import Exceptions
from TemperatureReading import TemperatureReading, DbTemperatureReading


class ApiClient(object):

    def __init__(self, url, key):
        self.url = url.strip('\\').strip('/')
        self.key = key
        self.apiKeyHeaderName = 'IoTApiKey'

    def get_location_id(self, location):
        url = self.url + '/api/iot/locations'
        headers = {self.apiKeyHeaderName: self.key, "Content-Type": "application/json"}
        response = requests.post(url, headers=headers, json="\"" + location + "\"")
        if response.status_code in (200, 201):
            return response.json()['id']

        if response.status_code in (401, 403):
            raise Exceptions.ApiKeyException("Invalid API key is specified.")

        raise Exceptions.NetworkException("Returned status code: {}. Validate specified url and api_key parameters.")

    def get_sensor_id(self, sensor):
        url = self.url + '/api/iot/sensors'
        headers = {self.apiKeyHeaderName: self.key, "Content-Type": "application/json"}
        response = requests.post(url, headers=headers, json="\"" + sensor + "\"")
        if response.status_code in (200, 201):
            return response.json()['id']

        if response.status_code in (401, 403):
            raise Exceptions.ApiKeyException("Invalid API key is specified.")

        raise Exceptions.NetworkException("Returned status code: {}. Validate specified url and api_key parameters.")

    def post_temperature_reading(self, temp_reading: TemperatureReading):
        url = self.url + '/api/iot/readings'
        body = [{"Temperature": temp_reading.temperature,
                 "TakenAt": temp_reading.taken_at.isoformat() + 'Z',
                 "LocationId": temp_reading.location_id,
                 "SensorId": temp_reading.sensor_id}]
        headers = {self.apiKeyHeaderName: self.key, "Content-Type": "application/json"}
        response = requests.post(url, headers=headers, json=body)
        return response.status_code

    def post_db_temperature_readings(self, db_temp_readings: Sequence[DbTemperatureReading]):
        url = self.url + '/api/iot/readings'
        body = []
        for db_temp_reading in db_temp_readings:
            body.append({"Temperature": db_temp_reading.temperature,
                         "TakenAt": db_temp_reading.taken_at.isoformat() + 'Z',
                         "LocationId": db_temp_reading.location_id,
                         "SensorId": db_temp_reading.sensor_id})
        headers = {self.apiKeyHeaderName: self.key, "Content-Type": "application/json"}
        response = requests.post(url, headers=headers, json=body)
        return response.status_code
