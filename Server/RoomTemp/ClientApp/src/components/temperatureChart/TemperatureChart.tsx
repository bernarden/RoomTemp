import React from "react";
import moment from "moment";
import Chart from "chart.js";

import "./TemperatureChart.css";
import { ITemperatureReadingsDto } from "../../interfaces/ITemperatureReadingsDto";
import { getTemperature } from "../../api/temperatureApi";
import { TempReadingRange } from "../../api/tempReadingRange";
import { ChartConfig } from "./ChartConfig";

interface IProps {
  selectedRange: TempReadingRange;
}

interface IState {
  selectedRange: TempReadingRange;
}

class TemperatureChart extends React.Component<IProps, IState> {
  private chart: Chart | undefined;
  private requestTimestamp: number = 1;

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
      this.requestTimestamp = this.requestTimestamp + 1;
      this.createChart();
      this.retrieveData();
      this.setState({ selectedRange: this.props.selectedRange });
    }
  }

  public render() {
    return <canvas id="temperatureChart" className="chart-container" />;
  }

  private retrieveData(): void {
    this.getTemperatureReadings(0);
    this.getTemperatureReadings(1);
    this.getTemperatureReadings(2);
    this.getTemperatureReadings(3);
  }

  private async getTemperatureReadings(retrievalIndex: number): Promise<void> {
    const today: Date = new Date();
    const dateToRetrieve: Date = new Date(
      today.setDate(today.getDate() - retrievalIndex * 7)
    );
    const dateToRetrieveAsString = moment(dateToRetrieve).format();
    
    const timestampBeforeRequest = this.requestTimestamp;
    const data: ITemperatureReadingsDto = await getTemperature(
      dateToRetrieveAsString,
      this.props.selectedRange
    );
    if (data && this.requestTimestamp === timestampBeforeRequest) {
      this.renderChart(data, retrievalIndex);
    }
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

    if (this.chart && this.chart.data && this.chart.data.datasets) {
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
    this.chart = new Chart(ctx, options);
  }
}
export default TemperatureChart;
