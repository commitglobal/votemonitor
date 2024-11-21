import { usePrevSearch } from '@/common/prev-search-store';
import type { FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { mapFormType } from '@/lib/utils';
import { formAggregatedDetailsQueryOptions, Route } from '@/routes/responses/$formId.aggregated';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { SubmissionType } from '../../models/common';
import type { Responder } from '../../models/form-submissions-aggregated';
import { AggregateCard } from '../AggregateCard/AggregateCard';

export default function FormSubmissionsAggregatedDetails(): FunctionComponent {
  const { formId } = Route.useParams();
  const prevSearch = usePrevSearch();
  const params = Route.useSearch();

  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const {
    data: {
      submissionsAggregate: { defaultLanguage, formCode, formType, aggregates, responders },
      attachments,
      notes,
    },
  } = useSuspenseQuery(formAggregatedDetailsQueryOptions(currentElectionRoundId, formId, params));

  const respondersAggregated = responders.reduce<Record<string, Responder>>(
    (grouped, responder) => ({
      ...grouped,
      [responder.responderId]: responder,
    }),
    {}
  );

  return (
    <Layout
      backButton={<NavigateBack search={prevSearch} to='/responses' />}
      breadcrumbs={
        <div className='flex flex-row gap-2 mb-4 breadcrumbs'>
          <Link search={prevSearch as any} className='crumb' to='/responses' preload='intent'>
            responses
          </Link>
          <Link className='crumb'>{formId}</Link>
        </div>
      }
      title={`${formCode} - ${mapFormType(formType)}`}>
      <div className='flex flex-col gap-10'>
        {Object.values(aggregates).map((aggregate) => {
          return (
            <AggregateCard
              key={aggregate.questionId}
              submissionType={SubmissionType.FormSubmission}
              aggregate={aggregate}
              language={defaultLanguage}
              responders={respondersAggregated}
              attachments={attachments}
              notes={notes}
            />
          );
        })}
      </div>
    </Layout>
  );
}
