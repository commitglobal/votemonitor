import type { FunctionComponent } from '@/common/types';
import type { Responder, TextQuestionAggregate } from '../../models/form-aggregated';
import { Link } from '@tanstack/react-router';
import { ArrowTopRightOnSquareIcon } from '@heroicons/react/24/outline';

type TextAggregateContentProps = {
  aggregate: TextQuestionAggregate;
  responders: Record<string, Responder>;
};

export function TextAggregateContent({ aggregate, responders }: TextAggregateContentProps): FunctionComponent {
  return (
    <div className='flex flex-col gap-4 max-h-96 overflow-auto'>
      {aggregate.answers.map(({ submissionId, responderId, value }, index) => (
        <div key={`${responderId}-${index}`}>
          <h3 className='font-bold'>{responders[responderId]?.firstName ?? ''} {' '} {responders[responderId]?.lastName ?? ''}{':'}</h3>
          <p>
            <Link
              search
              className='text-purple-500 font-bold flex gap-1'
              to='/responses/$submissionId'
              params={{ submissionId }}
              preload={false}>
              {submissionId.slice(0, 8)}
              <ArrowTopRightOnSquareIcon className='w-4' />
            </Link>
          </p>
          <p>{value}</p>
          <hr />
        </div>
      ))}
    </div>
  );
}
