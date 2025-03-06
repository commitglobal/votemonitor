import { ratingScaleToNumber, round } from '@/lib/utils';
import { BarElement, CategoryScale, Chart as ChartJS, ChartData, Legend, LinearScale, Title, Tooltip } from 'chart.js';
import { forwardRef } from 'react';
import { Bar } from 'react-chartjs-2';

import { purple500 } from '../../utils/chart-colors';

import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { RatingQuestionAggregate } from '../../models/common';

type RatingAggregateContentProps = {
  aggregate: RatingQuestionAggregate;
  language: string;
};

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

const RatingAggregateContent = forwardRef<ChartJSOrUndefined<'bar', number[]>, RatingAggregateContentProps>(
  ({ aggregate, language }, ref) => {
    const scale = ratingScaleToNumber(aggregate.question.scale);
    const labels = Array.from({ length: scale }, (_, i) => i + 1);
    const dataset = labels.map((l) => aggregate.answersHistogram[l] ?? 0);

    const data: ChartData<'bar', number[]> = {
      labels: labels,
      datasets: [{ data: dataset }],
    };

    return (
      <>
        <div className='flex flex-col gap-1'>
          <div>
            <span className='mr-1 font-bold'>Min:</span>
            <span>{aggregate.min}</span>
          </div>
          <div>
            <span className='mr-1 font-bold'>Max:</span>
            <span> {aggregate.max}</span>
          </div>
          <div>
            <span className='mr-1 font-bold'>Average:</span>
            <span>{round(aggregate.average, 2)}</span>
          </div>
        </div>

        <div className='h-[300px]'>
          <Bar
            ref={ref}
            data={data}
            options={{
              maintainAspectRatio: false,
              datasets: { bar: { barThickness: 25, backgroundColor: purple500 } },
              plugins: {
                legend: { display: false },
                datalabels: {
                  align: 'top',
                  anchor: 'end',
                  formatter: (value) =>
                    value ? `${value} (${round((value / aggregate.answersAggregated) * 100, 2)}%)` : '0',
                },
              },
              scales: {
                y: { max: Math.round(Math.max(...dataset) * 1.2) },
                x: {
                  min: 1,
                  max: scale,
                  ticks: {
                    callback: function (value) {
                      const displayValue = Number(value) + 1;

                      if (displayValue === 1) {
                        const lowerLabel = aggregate.question.lowerLabel?.[language];
                        return lowerLabel ? `${displayValue} (${lowerLabel})` : `${displayValue}`;
                      }

                      if (displayValue === scale) {
                        const upperLabel = aggregate.question.upperLabel?.[language];
                        return upperLabel ? `${displayValue} (${upperLabel})` : `${displayValue}`;
                      }

                      return displayValue;
                    },
                  },
                },
              },
            }}
          />
        </div>
      </>
    );
  }
);
export { RatingAggregateContent };
