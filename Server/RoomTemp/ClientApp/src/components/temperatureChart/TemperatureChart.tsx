import * as React from "react";
import * as moment from "moment";
import * as Chart from "chart.js";

import "./TemperatureChart.css";
import { ITemperatureReadingDto } from "src/interfaces/ITemperatureReadingDto";
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

  private getTemperatureReadings(retrivalIndex: number): void {
    const today: Date = new Date();
    const dateToRetrieve: Date = new Date(
      today.setDate(today.getDate() - retrivalIndex * 7)
    );
    const dateToRetrieveAsString = moment(dateToRetrieve).format();

    getTemperature(dateToRetrieveAsString, this.props.selectedRange).then(
      (data: ITemperatureReadingDto[]) => {
        if (data.length === 0) {
          return;
        }
        
        this.renderChart(data, retrivalIndex);
      }
    );
  }

  private renderChart(
    retrievedData: ITemperatureReadingDto[],
    retrivalIndex: number
  ) {
    const dataSet: Chart.ChartDataSets = ChartConfig.GenerateChartDataset(
      this.props.selectedRange,
      retrivalIndex,
      retrievedData
    );
    (dataSet as any).SortingIndex = retrivalIndex;

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
    const options = ChartConfig.GetChartOptions(this.props.selectedRange);
    const ctx: HTMLCanvasElement = document.getElementById(
      "temperatureChart"
    ) as HTMLCanvasElement;
    // tslint:disable-next-line:no-unused-expression
    this.chart = new Chart(ctx, options);
  }
}
export default TemperatureChart;
