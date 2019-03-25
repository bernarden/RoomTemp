import * as React from "react";
import * as moment from "moment";
import * as Chart from "chart.js";

import "./TemperatureChart.css";
import { ITemperatureReadingsDto } from "src/interfaces/ITemperatureReadingsDto";
import { getTemperature } from "src/api/temperatureApi";
import { TempReadingRange } from "src/api/tempReadingRange";
import { ChartConfig } from "./ChartConfig";

interface IProps {
  selectedRange: TempReadingRange;
}

interface IState {
  selectedRange: TempReadingRange;
}

class TemperatureChart extends React.Component<IProps, IState> {
  public chart: Chart;
  public readingInterval: number = 1;

  constructor(props: any) {
    super(props);
    this.state = { selectedRange: props.selectedRange };
  }

  public componentDidMount() {
    this.createChart();
    this.retrieveData();
  }

  public componentDidUpdate() {
    if (this.props.selectedRange !== this.state.selectedRange) {
      this.createChart();
      this.retrieveData();
      this.setState({ selectedRange: this.props.selectedRange });
    }
  }

  public render() {
    return <canvas id="temperatureChart" className="chart-container" />;
  }

  private retrieveData(): any {
    this.getTemperatureReadings(0);
    this.getTemperatureReadings(1);
    this.getTemperatureReadings(2);
    this.getTemperatureReadings(3);
  }

  private getTemperatureReadings(retrievalIndex: number): void {
    const today: Date = new Date();
    const dateToRetrieve: Date = new Date(
      today.setDate(today.getDate() - retrievalIndex * 7)
    );
    const dateToRetrieveAsString = moment(dateToRetrieve).format();

    getTemperature(dateToRetrieveAsString, this.props.selectedRange).then(
      (data: ITemperatureReadingsDto) => {
        this.renderChart(data, retrievalIndex);
      }
    );
  }

  private renderChart(
    retrievedData: ITemperatureReadingsDto,
    retrievalIndex: number
  ) {
    const dataSet: Chart.ChartDataSets = ChartConfig.generateChartDataset(
      this.props.selectedRange,
      retrievalIndex,
      retrievedData
    );
    (dataSet as any).SortingIndex = retrievalIndex;

    if (this.chart.data && this.chart.data.datasets) {
      this.chart.data.datasets.push(dataSet);
      this.chart.data.datasets.sort(
        (x, y) => (x as any).SortingIndex - (y as any).SortingIndex
      );
      this.chart.update();
    }
  }

  private createChart() {
    if (this.chart) {
      this.chart.data.datasets = [];
    }
    const options = ChartConfig.getChartOptions(this.props.selectedRange);
    const ctx: HTMLCanvasElement = document.getElementById(
      "temperatureChart"
    ) as HTMLCanvasElement;
    // tslint:disable-next-line:no-unused-expression
    this.chart = new Chart(ctx, options);
  }
}
export default TemperatureChart;
