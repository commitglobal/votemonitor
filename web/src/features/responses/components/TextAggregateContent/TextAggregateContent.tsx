import type { FunctionComponent } from '@/common/types';
import type { Responder } from '../../models/form-aggregated';
import { Link } from '@tanstack/react-router';
import { ArrowTopRightOnSquareIcon } from '@heroicons/react/24/outline';
import { SubmissionType, TextQuestionAggregate } from '../../models/common';

type TextAggregateContentProps = {
  submissionType: SubmissionType;
  aggregate: TextQuestionAggregate;
  responders?: Record<string, Responder>;
};

export function TextAggregateContent({
  submissionType,
  aggregate,
  responders,
}: TextAggregateContentProps): FunctionComponent {
  return (
    <div className='flex flex-col gap-4 overflow-auto max-h-96'>
      {aggregate.answers.map(({ submissionId, responderId, value }, index) => (
        <div key={`${responderId}-${index}`}>
          {responders && (
            <h3 className='font-bold'>
              {responders[responderId]?.firstName ?? ''} {responders[responderId]?.lastName ?? ''}
              {':'}
            </h3>
          )}
          <p>

            {submissionType === SubmissionType.CitizenReport ? (
              <Link
                search
                className='flex gap-1 font-bold text-purple-500'
                to='/responses/citizen-reports/$citizenReportId'
                params={{ citizenReportId: submissionId }}
                preload={false}
                target='_blank'>
                {submissionId.slice(0, 8)}
                <ArrowTopRightOnSquareIcon className='w-4' />
              </Link>
            ) : (
              <Link
                search
                className='flex gap-1 font-bold text-purple-500'
                to='/responses/$submissionId'
                params={{ submissionId }}
                preload={false}
                target='_blank'>
                {submissionId.slice(0, 8)}
                <ArrowTopRightOnSquareIcon className='w-4' />
              </Link>
            )}
          </p>
          <p>{value}</p>
          <hr />
        </div>
      ))}
    </div>
  );
}
