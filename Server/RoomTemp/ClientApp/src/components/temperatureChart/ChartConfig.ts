import moment from "moment";
import { ChartConfiguration, ChartDataset, TimeUnit} from "chart.js";
import { Chart, LineController, LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Filler, Legend, TimeScale  } from 'chart.js';

import { TempReadingRange } from "../../api/tempReadingRange";
import { ITemperatureReadingsDto } from "../../interfaces/ITemperatureReadingsDto";

// TODO: Do we need to register all of these?
Chart.register(LineController, LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Filler, Legend, TimeScale);

export interface TemperatureDataPoint {
  x: Date;
  y: number;
}


export class ChartConfig {
  public static getChartOptions(range: TempReadingRange) {
    let specifiedUnit: TimeUnit | undefined;
    let specifiedDisplayFormats;
    if (range === TempReadingRange.Week) {
      specifiedUnit = "day";
      specifiedDisplayFormats = { day: "dddd" };
    } else if (range === TempReadingRange.Day) {
      specifiedUnit = "hour";
      specifiedDisplayFormats = { day: "HH:mm" };
    }

    const options: ChartConfiguration = {
      type: "line",
      data: {
        datasets: []
      },
      options: {
        scales: {
          x: {
            type: "time",
            time: {
              unit: specifiedUnit,
              displayFormats: specifiedDisplayFormats
            },
          }
        }
      }
    };
    return options;
  }

  public static generateChartDataset(
    range: TempReadingRange,
    retrievalIndex: number,
    retrievedData: ITemperatureReadingsDto
  ): ChartDataset {
    const processedData: TemperatureDataPoint[] = retrievedData.temperatures.map((x: any) => {
      return {
        x: moment(new Date(x.takenAt))
          .add(retrievalIndex, "w")
          .toDate(),
        y: x.temperature
      };
    });

    let datasetLabel: string
    if (retrievalIndex === 0) {
      datasetLabel = this.getCurrentPeriodName(range)
    } else {
      datasetLabel = new Date(retrievedData.searchStartDateTime).toLocaleDateString("en-NZ")
    }
  
    const dataSet: ChartDataset = {
      data: processedData as any,
      type: "line",
      borderWidth: 3,
      label: datasetLabel,
      fill: false,
      backgroundColor: this.getColour(retrievalIndex, false),
      borderColor: this.getColour(retrievalIndex, true),
      borderCapStyle: "butt",
      borderJoinStyle: "miter",
      pointBackgroundColor: this.getColour(retrievalIndex, false),
      pointBorderColor: this.getColour(retrievalIndex, true),
      pointBorderWidth: 1,
      pointHoverRadius: 5,
      pointHoverBackgroundColor: this.getColour(retrievalIndex, false),
      pointHoverBorderColor: this.getColour(retrievalIndex, true),
      pointHoverBorderWidth: 2,
      pointRadius: 1,
      tension: 0.5,
      pointHitRadius: 10,
      cubicInterpolationMode: "default"
    };
    return dataSet;
  }

  private static getCurrentPeriodName(range: TempReadingRange): string {
    switch (range) {
      case TempReadingRange.Hour:
        return "This hour";
      case TempReadingRange.Day:
        return "Today";
      case TempReadingRange.Week:
        return "Current Week";
      case TempReadingRange.Month:
        return "Current Month";
      default:
        return "";
    }
  }

  private static getColour(retrievalIndex: number, isBorder: boolean): string {
    switch (retrievalIndex) {
      case 0:
        return isBorder ? "rgba(255, 99, 132, 0.8)" : "rgba(255, 99, 132, 1)";
      case 1:
        return isBorder ? "rgba(54, 162, 235, 0.8)" : "rgba(54, 162, 235, 1)";
      case 2:
        return isBorder ? "rgba(255, 206, 86, 0.8)" : "rgba(255, 206, 86, 1)";
      case 3:
        return isBorder ? "rgba(75, 192, 192, 0.8)" : "rgba(75, 192, 192, 1)";
      case 4:
        return isBorder ? "rgba(153, 102, 255, 0.8)" : "rgba(153, 102, 255, 1)";
      case 5:
        return isBorder ? "rgba(255, 159, 64, 0.8)" : "rgba(255, 159, 64, 1)";
    }

    return "#ffffff";
  }
}
