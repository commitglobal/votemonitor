import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
  type ChartData,
} from 'chart.js';
import { forwardRef } from 'react';
import { Bar } from 'react-chartjs-2';
import type { FunctionComponent } from '@/common/types';
import type { MultiSelectQuestionAggregate } from '../../models/form-aggregated';
import { purple500 } from '../../utils/chart-colors';

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
      datasets: [{ data: dataset }],
    };

    return (
      <div className='h-[200px]'>
        <Bar
          ref={ref}
          data={data}
          options={{
            maintainAspectRatio: false,
            datasets: { bar: { barThickness: 10, backgroundColor: purple500 } },
            indexAxis: 'y',
            plugins: {
              legend: { display: false },
              datalabels: {
                align: 'right',
                anchor: 'end',
                formatter: (value) => (value ? `— ${value} (${(value / aggregate.answersAggregated) * 100}%)` : '0'),
              },
            },
            scales: { x: { max: Math.max(...dataset) * 1.2 } },
          }}
        />
      </div>
    );
  }
);

export { MultiSelectAggregateContent };
