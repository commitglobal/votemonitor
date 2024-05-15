import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';
import { forwardRef } from 'react';
import { Bar } from 'react-chartjs-2';
import type { NumberQuestionAggregate } from '../../models/form-aggregated';
import { purple500 } from '../../utils/chart-colors';
import { round } from '@/lib/utils';
import { FunctionComponent } from '@/common/types';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

type NumberAggregateContentProps = {
  aggregate: NumberQuestionAggregate;
};

export function NumberAggregateContent({ aggregate }: NumberAggregateContentProps): FunctionComponent {
  return (
    <div className='flex flex-col gap-1'>
      <div><span className='font-bold mr-1'>Min:</span><span>{aggregate.min}</span></div>
      <div><span className='font-bold mr-1'>Min:</span><span> {aggregate.max}</span></div>
      <div><span className='font-bold mr-1'>Average:</span><span>{round(aggregate.average, 2)}</span></div>
    </div>
  );
}

