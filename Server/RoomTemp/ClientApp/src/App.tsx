import * as React from 'react';
import './App.css';
import logo from './logo.svg';
import TemperatureChart from './components/temperatureChart/TemperatureChart';

class App extends React.Component {
  constructor(props: any) {
    super(props);
  }

  public render() {
    return (
      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <h1 className="App-title">Welcome to your Temperature Reading</h1>
        </header>

        <TemperatureChart />
      </div>
    );
  }
}
export default App;
