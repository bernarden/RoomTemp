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
    private selectedRangeLocalStorageKey = 'LastSelectedRange';

    constructor(props: any) {
        super(props);
        this.state = { selectedRange: this.getDefaultOrLastSelectedRange() };
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
        localStorage.setItem(this.selectedRangeLocalStorageKey, String(range));
        this.setState({ selectedRange: range });
    };

    private getDefaultOrLastSelectedRange() : TempReadingRange {
        const lastSelectedRange = localStorage.getItem(this.selectedRangeLocalStorageKey);
        if (lastSelectedRange) {
            return Number(lastSelectedRange);
        }

        localStorage.setItem(this.selectedRangeLocalStorageKey, String(TempReadingRange.Week));
        return TempReadingRange.Week;
    }
}


export default App;