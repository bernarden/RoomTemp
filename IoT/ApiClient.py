import requests


class ApiClient(object):

    def __init__(self, url, key):
        self.url = url.strip('\\').strip('/')
        self.key = key
        self.apiKeyHeaderName = 'IoTApiKey'

    def get_location_id(self, location):
        url = self.url + '/api/iot/locations'
        headers = {self.apiKeyHeaderName: self.key, "Content-Type": "application/json"}
        response = requests.post(url, headers=headers, json="\"" + location + "\"")
        return response.json()['id']

    def get_sensor_id(self, sensor):
        url = self.url + '/api/iot/sensors'
        headers = {self.apiKeyHeaderName: self.key, "Content-Type": "application/json"}
        response = requests.post(url, headers=headers, json="\"" + sensor + "\"")
        return response.json()['id']

    def post_temperature_reading(self, temperature, date, location_id, sensor_id):
        url = self.url + '/api/iot/readings'
        body = [{"Temperature": temperature, "TakenAt": date, "LocationId": location_id, "SensorId": sensor_id}]
        headers = {self.apiKeyHeaderName: self.key, "Content-Type": "application/json"}
        response = requests.post(url, headers=headers, json=body)
        return response.status_code
