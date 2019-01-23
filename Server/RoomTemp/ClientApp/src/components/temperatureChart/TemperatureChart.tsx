import * as React from 'react';
import './TemperatureChart.css';
import * as Chart from 'chart.js';
import { ITemperatureReadingDto } from 'src/interfaces/ITemperatureReadingDto';
import { getMonday } from 'src/helpers/DateHelper';
import { getTemperature } from 'src/api/temperatureApi';
import * as moment from 'moment';
class TemperatureChart extends React.Component {
  public chart: Chart;
  public readingInterval: number = 100;

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

    const thisMonday = getMonday(monday).toDateString();
    getTemperature(thisMonday).then((data: ITemperatureReadingDto[]) => {
      this.renderChart(data, weeksBack);
    });
  }

  public getColour(week: number): string {
    let colour: string = 'rgba(75,192,192,0.4)';
    switch (week) {
      case 0:
        colour = '#f4ffff';
        break;
      case 1:
        colour = 'rgba(75,192,192, 1)';
        break;
      case 2:
        colour = 'rgba(75,192,192, .5)';
        break;
      case 3:
        colour = 'rgba(75,192,192, 0.1)';
        break;
    }

    return colour;
  }

  public renderChart(readings: ITemperatureReadingDto[], weekNumber: number) {
    if (!this.chart) {
      this.createChart();
    }

    const workedData = readings.filter(
      (x: any, index: number) => index % this.readingInterval === 0
    );
    const processedData = workedData.map((x: any) => {
      return {
        t: moment(new Date(x.takenAt))
          .add(weekNumber, 'w')
          .toDate(),
        y: x.temperature
      };
    });

    const datasetLabel =
      weekNumber === 0 ? 'Current Week' : processedData[0] ? processedData[0].t.getDate() + '' : '';

    const dataSet: Chart.ChartDataSets = {
      data: processedData,
      type: 'line',
      borderWidth: 3,
      label: datasetLabel,
      fill: false,
      lineTension: 0.1,
      backgroundColor: this.getColour(weekNumber),
      borderColor: this.getColour(weekNumber),
      borderCapStyle: 'butt',
      borderJoinStyle: 'miter',
      pointBorderColor: this.getColour(weekNumber),
      pointBackgroundColor: this.getColour(weekNumber),
      pointBorderWidth: 1,
      pointHoverRadius: 5,
      pointHoverBackgroundColor: this.getColour(weekNumber),
      pointHoverBorderColor: this.getColour(weekNumber),
      pointHoverBorderWidth: 2,
      pointRadius: 1,
      pointHitRadius: 10,
      cubicInterpolationMode: 'default'
    };

    if (this.chart.data && this.chart.data.datasets) {
      this.chart.data.datasets.push(dataSet);
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
