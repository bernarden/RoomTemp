import * as React from 'react';
import * as moment from 'moment';
import * as Chart from 'chart.js';

import { getMonday } from 'src/helpers/DateHelper';
import './TemperatureChart.css';
import { ITemperatureReadingDto } from 'src/interfaces/ITemperatureReadingDto';
import { getTemperature } from 'src/api/temperatureApi';

class TemperatureChart extends React.Component {
  public chart: Chart;
  public readingInterval: number = 1;

  constructor(props: any) {
    super(props);
  }

  public componentDidMount() {
    this.getTemperatureReadings(0);
    this.getTemperatureReadings(1);
    this.getTemperatureReadings(2);
    this.getTemperatureReadings(3);
  }

  public getTemperatureReadings(weeksBack: number): void {
    const today = new Date();
    const monday = new Date(today.setDate(today.getDate() - weeksBack * 7));

    const thisMonday = moment(getMonday(monday)).format();
    getTemperature(thisMonday).then((data: ITemperatureReadingDto[]) => {
      this.renderChart(data, weeksBack);
    });
  }

  public getColour(week: number, isBorder: boolean): string {
      switch (week) {
          case 0:
              return isBorder ? "rgba(255, 99, 132, 0.8)" : "rgba(255, 99, 132, 1)";
          case 1:
              return isBorder ? 'rgba(54, 162, 235, 0.8)' : 'rgba(54, 162, 235, 1)';
          case 2:
              return isBorder ? 'rgba(255, 206, 86, 0.8)' : 'rgba(255, 206, 86, 1)';
          case 3:
              return isBorder ? 'rgba(75, 192, 192, 0.8)' : 'rgba(75, 192, 192, 1)';
          case 4:
              return isBorder ? 'rgba(153, 102, 255, 0.8)' : 'rgba(153, 102, 255, 1)';
          case 5:
              return isBorder ? 'rgba(255, 159, 64, 0.8)' : 'rgba(255, 159, 64, 1)';
          // case 6:
          //    return isBorder ? "rgba(255, 99, 132, 0.8)" : "rgba(255, 99, 132, 1)";
      }

      return '#ffffff';
  }

  public renderChart(readings: ITemperatureReadingDto[], weekNumber: number) {
    if (!this.chart) {
      this.createChart();
    }

    const workedData = readings.filter((x: any, index: number) => index % this.readingInterval === 0);
    const processedData = workedData.map((x: any) => {
      return {
        t: moment(new Date(x.takenAt))
            .add(weekNumber, 'w')
            .toDate(),
        y: x.temperature
      };
    });

    const datasetLabel = weekNumber === 0 ? 'Current Week' : workedData[0] ? new Date(workedData[0].takenAt).toLocaleDateString('en-NZ') + '' : '';

    const dataSet: Chart.ChartDataSets = {
      data: processedData,
      type: 'line',
      borderWidth: 3,
      label: datasetLabel,
      fill: false,
      lineTension: 0.1,
      backgroundColor: this.getColour(weekNumber, false),
      borderColor: this.getColour(weekNumber, true),
      borderCapStyle: 'butt',
      borderJoinStyle: 'miter',
      pointBackgroundColor: this.getColour(weekNumber,false),
      pointBorderColor: this.getColour(weekNumber, true),
      pointBorderWidth: 1,
      pointHoverRadius: 5,
      pointHoverBackgroundColor: this.getColour(weekNumber, false),
      pointHoverBorderColor: this.getColour(weekNumber, true),
      pointHoverBorderWidth: 2,
      pointRadius: 1,
      pointHitRadius: 10,
      cubicInterpolationMode: 'default'
    };

    (dataSet as any).SortingIndex = weekNumber;

    if (this.chart.data && this.chart.data.datasets) {
      this.chart.data.datasets.push(dataSet);
      this.chart.data.datasets.sort((x, y) => (x as any).SortingIndex - (y as any).SortingIndex);
      this.chart.update();
    }
  }

  public createChart() {
    const options: Chart.ChartConfiguration = {
      type: 'line',
      data: {
        datasets: []
      },
      options: {
        scales: {
          type: 'time',
          xAxes: [
            {
              type: 'time',
              time: {
                unitStepSize: 1,
                unit: 'day',
                displayFormats: {
                  millisecond: 'DD MMM',
                  second: 'DD MMM',
                  minute: 'DD MMM',
                  hour: 'DD MMM',
                  day: 'dddd',
                  week: 'DD MMM',
                  month: 'DD MMM',
                  quarter: 'DD MMM',
                  year: 'DD MMM'
                }
              },
              gridLines: {
                display: true
              }
            }
          ]
        }
      }
    };

    const ctx: HTMLCanvasElement = document.getElementById('temperatureChart') as HTMLCanvasElement;
    // tslint:disable-next-line:no-unused-expression
    this.chart = new Chart(ctx, options);
  }

  public render() {
    return <canvas id="temperatureChart" className="chart-container" />;
  }
}
export default TemperatureChart;
