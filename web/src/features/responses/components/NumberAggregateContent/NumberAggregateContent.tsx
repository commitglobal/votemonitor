import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';
import { forwardRef } from 'react';
import { Bar } from 'react-chartjs-2';
import type { NumberQuestionAggregate } from '../../models/form-aggregated';
import { purple500 } from '../../utils/chart-colors';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

type NumberAggregateContentProps = {
  aggregate: NumberQuestionAggregate;
};

const NumberAggregateContent = forwardRef<ChartJSOrUndefined<'bar', number[]>, NumberAggregateContentProps>(
  ({ aggregate }, ref) => {
    const dataset = aggregate.answers.reduce<Record<string, number>>((data, { value }) => {
      return { ...data, [value]: (data?.[value] ?? 0) + 1 };
    }, {});

    return (
      <div className='h-[200px]'>
        <Bar
          ref={ref}
          data={{
            labels: Object.keys(dataset),
            datasets: [
              {
                data: Object.values(dataset),
                borderColor: purple500,
                borderWidth: 1,
              },
            ],
          }}
          options={{
            datasets: { bar: { barThickness: 10, backgroundColor: purple500 } },
            maintainAspectRatio: false,
            plugins: {
              legend: { display: false },
              datalabels: {
                align: 'top',
                anchor: 'end',
              },
            },
            scales: { y: { max: Math.max(...Object.values(dataset)) * 1.2 } },
          }}
        />
      </div>
    );
  }
);

export { NumberAggregateContent };
