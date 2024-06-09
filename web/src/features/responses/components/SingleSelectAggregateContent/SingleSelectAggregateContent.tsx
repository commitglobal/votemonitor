import { round } from '@/lib/utils';
import { ArcElement, Chart as ChartJS, Legend, Tooltip } from 'chart.js';
import { forwardRef } from 'react';
import { Pie } from 'react-chartjs-2';

import { getColorsForSelectChart } from '../../utils/chart-colors';

import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import type { FunctionComponent } from '@/common/types';
import type { SingleSelectQuestionAggregate } from '../../models/form-aggregated';
ChartJS.register(ArcElement, Tooltip, Legend);

type SingleSelectAggregateContentProps = {
  aggregate: SingleSelectQuestionAggregate;
  language: string;
};

const SingleSelectAggregateContent = forwardRef<ChartJSOrUndefined<'pie', number[]>, SingleSelectAggregateContentProps>(
  ({ aggregate, language }, ref): FunctionComponent => {
    return (
      <div className='h-96 grid grid-cols-2'>
        <Pie
          ref={ref}
          data={{
            labels: aggregate.question.options.map((option) => option.text[language]),
            datasets: [
              {
                data: aggregate.question.options.map((option) => aggregate.answersHistogram[option.id] ?? 0),
                borderWidth: (ctx,o) => ctx.dataset.data[ctx.dataIndex]=== 0 ? 0 : 1,
                backgroundColor: getColorsForSelectChart(aggregate.question.options, false),
                hoverBackgroundColor: getColorsForSelectChart(aggregate.question.options, true)
              },
            ],
          }}
          options={{
            maintainAspectRatio: false,
            devicePixelRatio: 1.5,
            plugins: {
              legend: {
                position: 'right' as const,
                align: 'center',
                onClick: () => { },
                labels: {
                  boxWidth: 20,
                  boxHeight: 20,
                  usePointStyle: true,
                  pointStyle: 'rect' as const,
                }
              },
              datalabels: {
                color: '#fff',
                formatter: (value) => (value ? `${value} (${round((value / aggregate.answersAggregated) * 100, 2) }%)` : ''),
              },
            },
          }}
        />
      </div>
    );
  }
);

export { SingleSelectAggregateContent };
