import axios from 'axios';

import { ITemperatureReadingDto } from 'src/interfaces/ITemperatureReadingDto';
import { TempReadingRange } from './tempReadingRange';

export const getTemperature = (startDate: string, range: TempReadingRange ): Promise<ITemperatureReadingDto[]> => {
  return axios
    .get(
      `/api/webclient/tempreadings?start=${encodeURIComponent(startDate)}&range=${range}&deviceId=1&locationId=3&sensorId=1`
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
