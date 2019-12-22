import React from 'react';
import classnames from 'classnames';

import { TempReadingRange } from '../../api/tempReadingRange';
import './RangeSelector.css';

interface IProps {
    selectedRange: TempReadingRange,
    updateRange:(range:TempReadingRange )=>void
}

class RangeSelector extends React.Component<IProps>{

    public render(){
        return (<div className="btn-group btn-group-toggle range-selector"  data-toggle="buttons">
                    {this.renderRangeButton(TempReadingRange.Day, 'Day')}
                    {this.renderRangeButton(TempReadingRange.Week, 'Week')}
                </div>);
    }

    private renderRangeButton(range: TempReadingRange, rangeDisplayName: string) {
        const classes = classnames('btn btn-secondary', { 'active': this.props.selectedRange === range});
        return (
            <label className={classes} >
                <input type="radio" name="options" id="option1" onClick={this.handleRangeSelectionClick(range)}/>
                {rangeDisplayName}
            </label>
        );
    }

    private handleRangeSelectionClick = (range: TempReadingRange) => (e: React.SyntheticEvent<any>): void => {
        this.props.updateRange(range);
    };
}

export default RangeSelector;