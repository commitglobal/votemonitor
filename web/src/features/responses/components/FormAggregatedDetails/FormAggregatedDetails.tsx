import type { FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { mapFormType } from '@/lib/utils';
import { formAggregatedDetailsQueryOptions, Route } from '@/routes/responses/$formId.aggregated';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Link, useRouter } from '@tanstack/react-router';
import type { Responder } from '../../models/form-aggregated';
import { AggregateCard } from '../AggregateCard/AggregateCard';

export default function FormAggregatedDetails(): FunctionComponent {
  const { state } = useRouter();

  const { formId } = Route.useParams()
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const { data: formSubmission } = useSuspenseQuery(formAggregatedDetailsQueryOptions(currentElectionRoundId, formId));

  const { submissionsAggregate } = formSubmission;
  const { defaultLanguage, formCode, formType, aggregates, responders } = submissionsAggregate;

  const groupedAttachments = responders.reduce<Record<string, Responder>>(
    (grouped, responder) => ({
      ...grouped,
      [responder.responderId]: responder,
    }),
    {}
  );

  return (
    <Layout
      backButton={<NavigateBack search={state.resolvedLocation.search} to='/responses' />}
      breadcrumbs={
        <div className='breadcrumbs flex flex-row gap-2 mb-4'>
          <Link search={state.resolvedLocation.search as any} className='crumb' to='/responses' preload='intent'>
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
              aggregate={aggregate}
              language={defaultLanguage}
              responders={groupedAttachments}
            />
          );
        })}
      </div>
    </Layout>
  );
}
