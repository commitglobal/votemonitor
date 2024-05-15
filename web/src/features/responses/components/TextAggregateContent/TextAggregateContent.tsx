import type { FunctionComponent } from '@/common/types';
import type { Responder, TextQuestionAggregate } from '../../models/form-aggregated';

type TextAggregateContentProps = {
  aggregate: TextQuestionAggregate;
  responders: Record<string, Responder>;
};

export function TextAggregateContent({ aggregate, responders }: TextAggregateContentProps): FunctionComponent {
  return (
    <div className='flex flex-col gap-4 max-h-96 overflow-auto'>
      {aggregate.answers.map(({ responder, value }, index) => (
        <div key={`${responder}-${index}`}>
          <h3 className='font-bold'>{responders[responder]?.firstName ?? ''} {' '} {responders[responder]?.lastName ?? ''}{':'}</h3>
          <p>{value}</p>
          <hr />
        </div>
      ))}
    </div>
  );
}
