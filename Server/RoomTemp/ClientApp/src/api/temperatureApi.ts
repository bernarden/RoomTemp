import axios from 'axios';

import { ITemperatureReadingDto } from 'src/interfaces/ITemperatureReadingDto';

export const getTemperature = (startDate: string): Promise<ITemperatureReadingDto[]> => {
  return axios
    .get(
      `/api/webclient/tempreadings?start=${encodeURIComponent(startDate)}&range=3&deviceId=1&locationId=3&sensorId=1`
    )
    .then(response => {
      if (response.status === 200) {
        return response.data;
      }
    })
    .catch(error => {
      // tslint:disable-next-line:no-console
      console.log('Server Error', error);
    });
};
