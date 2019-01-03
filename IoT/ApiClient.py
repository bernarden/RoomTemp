import requests

import Exceptions


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

    def post_temperature_reading(self, temperature, date, location_id, sensor_id):
        url = self.url + '/api/iot/readings'
        body = [{"Temperature": temperature, "TakenAt": date, "LocationId": location_id, "SensorId": sensor_id}]
        headers = {self.apiKeyHeaderName: self.key, "Content-Type": "application/json"}
        response = requests.post(url, headers=headers, json=body)
        return response.status_code
