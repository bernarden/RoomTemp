import * as moment from "moment";
import { ChartPoint, TimeDisplayFormat, TimeUnit } from "chart.js";

import { TempReadingRange } from "src/api/tempReadingRange";
import { ITemperatureReadingDto } from "src/interfaces/ITemperatureReadingDto";

export class ChartConfig {
  public static GetChartOptions(range: TempReadingRange) {
    let specifiedUnit: TimeUnit | undefined;
    let specifiedDisplayFormats: TimeDisplayFormat | undefined;
    if (range === TempReadingRange.Week) {
      specifiedUnit = "day";
      specifiedDisplayFormats = { day: "dddd" };
    } else if (range === TempReadingRange.Day) {
      specifiedUnit = "hour";
      specifiedDisplayFormats = { day: "HH:mm" };
    }

    const options: Chart.ChartConfiguration = {
      type: "line",
      data: {
        datasets: []
      },
      options: {
        scales: {
          type: "time",
          xAxes: [
            {
              type: "time",
              time: {
                unitStepSize: 1,
                unit: specifiedUnit,
                displayFormats: specifiedDisplayFormats
              },
              gridLines: {
                display: true
              }
            }
          ]
        }
      }
    };
    return options;
  }

  public static GenerateChartDataset(
    range: TempReadingRange,
    retrivalIndex: number,
    retrievedData: ITemperatureReadingDto[]
  ): Chart.ChartDataSets {
    const processedData: ChartPoint[] = retrievedData.map((x: any) => {
      return {
        t: moment(new Date(x.takenAt))
          .add(retrivalIndex, "w")
          .toDate(),
        y: x.temperature
      };
    });

    const datasetLabel: string = retrivalIndex === 0 ? this.getCurrentPeriodName(range) : new Date(retrievedData[0].takenAt).toLocaleDateString("en-NZ");
  
    const dataSet: Chart.ChartDataSets = {
      data: processedData,
      type: "line",
      borderWidth: 3,
      label: datasetLabel,
      fill: false,
      lineTension: 0.1,
      backgroundColor: this.getColour(retrivalIndex, false),
      borderColor: this.getColour(retrivalIndex, true),
      borderCapStyle: "butt",
      borderJoinStyle: "miter",
      pointBackgroundColor: this.getColour(retrivalIndex, false),
      pointBorderColor: this.getColour(retrivalIndex, true),
      pointBorderWidth: 1,
      pointHoverRadius: 5,
      pointHoverBackgroundColor: this.getColour(retrivalIndex, false),
      pointHoverBorderColor: this.getColour(retrivalIndex, true),
      pointHoverBorderWidth: 2,
      pointRadius: 1,
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

  private static getColour(retrivalIndex: number, isBorder: boolean): string {
    switch (retrivalIndex) {
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