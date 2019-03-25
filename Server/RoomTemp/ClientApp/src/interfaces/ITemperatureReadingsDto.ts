import { ITemperatureReadingDto } from './ITemperatureReadingDto';

export interface ITemperatureReadingsDto {
  temperatures: ITemperatureReadingDto[];
  searchStartDateTime: string;
  searchEndDateTime: string;
}
