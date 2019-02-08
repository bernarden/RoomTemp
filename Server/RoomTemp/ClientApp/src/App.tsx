import * as React from 'react';
import * as classnames from 'classnames';

import './App.css';
import logo from './logo.svg';
import TemperatureChart from './components/temperatureChart/TemperatureChart';
import { TempReadingRange } from './api/tempReadingRange';

interface IState {
    selectedRange: TempReadingRange
}

class App extends React.Component<{}, IState> {
    constructor(props: any) {
        super(props);
        this.state = { selectedRange: TempReadingRange.Week };
    }

    public render() {
        const rangeButtons = this.renderRangeButtons();
        return (
            <div className="App">
                <header className="App-header">
                    <img src={logo} className="App-logo" alt="logo"/>
                    <h1 className="App-title">Temperature Readings</h1>
                </header>

                {rangeButtons}

                <TemperatureChart/>
            </div>
        );
    }

    private renderRangeButtons() {
        return (
            <div className="btn-group btn-group-toggle" style={{paddingBottom:'10px'}} data-toggle="buttons">
                {this.renderRangeButton(TempReadingRange.Day, 'Day')}
                {this.renderRangeButton(TempReadingRange.Week, 'Week')}
            </div>);
    }

    private renderRangeButton(range: TempReadingRange, rangeDisplayName: string) {
        const classes = classnames('btn btn-secondary', { 'active': this.state.selectedRange === range});
        return (
            <label className={classes}>
                <input type="radio" name="options" id="option1" onClick={this.handleRangeSelectionClick(range)}/>
                {rangeDisplayName}
            </label>
        );
    }

    private handleRangeSelectionClick = (range: TempReadingRange) => (e: React.SyntheticEvent<any>): void => {
        this.setState({ selectedRange: range });
    };
}


export default App;