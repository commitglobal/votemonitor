import { FunctionComponent } from '@/common/types';
import { round } from '@/lib/utils';
import { BarElement, CategoryScale, Chart as ChartJS, Legend, LinearScale, Title, Tooltip } from 'chart.js';
import { NumberQuestionAggregate } from '../../models/common';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

type NumberAggregateContentProps = {
  aggregate: NumberQuestionAggregate;
};

export function NumberAggregateContent({ aggregate }: NumberAggregateContentProps): FunctionComponent {
  return (
    <div className='flex flex-col gap-1'>
      <div><span className='mr-1 font-bold'>Min:</span><span>{aggregate.min}</span></div>
      <div><span className='mr-1 font-bold'>Max:</span><span> {aggregate.max}</span></div>
      <div><span className='mr-1 font-bold'>Average:</span><span>{round(aggregate.average, 2)}</span></div>
    </div>
  );
}

