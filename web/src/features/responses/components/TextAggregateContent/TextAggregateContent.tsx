import type { FunctionComponent } from '@/common/types';
import type { TextQuestionAggregate } from '../../models/form-aggregated';

type TextAggregateContentProps = {
  aggregate: TextQuestionAggregate;
};

export function TextAggregateContent({ aggregate }: TextAggregateContentProps): FunctionComponent {
  return (
    <div className='flex flex-col gap-4 max-h-96 overflow-auto'>
      {aggregate.answers.map(({ responder, value }, index) => (
        <p key={`${responder}-${index}`}>{value}</p>
      ))}
    </div>
  );
}
