import { round } from '@/lib/utils';
import { BarElement, CategoryScale, Chart as ChartJS, ChartData, Legend, LinearScale, Title, Tooltip } from 'chart.js';
import { forwardRef } from 'react';
import { Bar } from 'react-chartjs-2';

import { getColorsForSelectChart } from '../../utils/chart-colors';

import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import type { FunctionComponent } from '@/common/types';
import { MultiSelectQuestionAggregate } from '../../models/common';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

type MultiSelectAggregateContentProps = {
  aggregate: MultiSelectQuestionAggregate;
  language: string;
};

const MultiSelectAggregateContent = forwardRef<ChartJSOrUndefined<'bar', number[]>, MultiSelectAggregateContentProps>(
  ({ aggregate, language }, ref): FunctionComponent => {
    const dataset = aggregate.question.options.map((option) => aggregate.answersHistogram[option.id] ?? 0);

    const data: ChartData<'bar', number[]> = {
      labels: aggregate.question.options.map((option) => option.text[language]),
      datasets: [
        {
          data: dataset
        }
      ],
    };

    return (
      <div className='h-[200px]'>
        <Bar
          ref={ref}
          data={data}
          options={{
            maintainAspectRatio: false,
            devicePixelRatio: 1.5,
            datasets: {
              bar: {
                barThickness: 10,
                backgroundColor: getColorsForSelectChart(aggregate.question.options, false),
                hoverBackgroundColor: getColorsForSelectChart(aggregate.question.options, true)
              }
            },
            indexAxis: 'y',
            plugins: {
              legend: { display: false },
              datalabels: {
                align: 'right',
                anchor: 'end',
                formatter: (value) => (value ? `— ${value} (${round((value / aggregate.answersAggregated) * 100, 2) }%)` : '0'),
              },
            },
            scales: { x: { max: Math.round(Math.max(...dataset) * 1.2) } },
          }}
        />
      </div>
    );
  }
);

export { MultiSelectAggregateContent };
