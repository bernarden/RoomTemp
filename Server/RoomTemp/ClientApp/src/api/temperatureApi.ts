import axios from 'axios';

import { ITemperatureReadingsDto } from 'src/interfaces/ITemperatureReadingsDto';
import { TempReadingRange } from './tempReadingRange';

export const getTemperature = (startDate: string, range: TempReadingRange ): Promise<ITemperatureReadingsDto> => {
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
