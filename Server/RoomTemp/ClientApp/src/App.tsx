import * as React from 'react';

import './App.css';
import logo from './logo.svg';
import TemperatureChart from './components/temperatureChart/TemperatureChart';
import { TempReadingRange } from './api/tempReadingRange';
import RangeSelector from './components/rangeSelector/RangeSelector';

interface IState {
    selectedRange: TempReadingRange
}

class App extends React.Component<{}, IState> {
    constructor(props: any) {
        super(props);
        this.state = { selectedRange: TempReadingRange.Week };
    }

    public render() {
        return (
            <div className="App">
                <header className="App-header">
                    <img src={logo} className="App-logo" alt="logo"/>
                    <h1 className="App-title">Temperature Readings</h1>
                </header>

                <RangeSelector selectedRange={this.state.selectedRange} updateRange={this.updateRangeSelection}/>

                <TemperatureChart selectedRange={this.state.selectedRange}/>
            </div>
        );
    }

    private updateRangeSelection = (range: TempReadingRange) => {
        this.setState({ selectedRange: range });
    };
}


export default App;