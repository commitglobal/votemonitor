import { FlagIcon } from '@heroicons/react/24/solid';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { forwardRef } from 'react';
import { Pie } from 'react-chartjs-2';
import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import type { FunctionComponent } from '@/common/types';
import { cn } from '@/lib/utils';
import type { SingleSelectQuestionAggregate } from '../../models/form-aggregated';
import { colors } from '../../utils/chart-colors';

ChartJS.register(ArcElement, Tooltip, Legend);

type SingleSelectAggregateContentProps = {
  aggregate: SingleSelectQuestionAggregate;
  language: string;
};

const SingleSelectAggregateContent = forwardRef<ChartJSOrUndefined<'pie', number[]>, SingleSelectAggregateContentProps>(
  ({ aggregate, language }, ref): FunctionComponent => {
    return (
      <div className='h-80 grid grid-cols-2'>
        <Pie
          ref={ref}
          data={{
            labels: aggregate.question.options.map((question) => question.text[language]),
            datasets: [
              {
                data: Object.values(aggregate.answersHistogram).map(
                  (value) => (value / aggregate.answersAggregated) * 100
                ),
                backgroundColor: colors,
              },
            ],
          }}
          options={{
            plugins: {
              legend: { display: false },
              datalabels: {
                color: '#fff',
                formatter(value) {
                  return `${value}%`;
                },
              },
            },
          }}
        />

        <div className='flex flex-col justify-center text-sm'>
          {aggregate.question.options.map((question, index) => (
            <div className='flex gap-1 items-center'>
              <span className='inline-block rounded-full h-2 w-2' style={{ backgroundColor: colors[index] }} />
              {question.text[language]}{' '}
              {question.isFlagged && (
                <span className='flex gap-1'>
                  (Flagged) <FlagIcon className={cn('text-destructive', 'w-4')} />
                </span>
              )}
            </div>
          ))}
        </div>
      </div>
    );
  }
);

export { SingleSelectAggregateContent };
