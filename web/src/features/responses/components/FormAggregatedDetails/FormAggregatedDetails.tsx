import { Link, useLoaderData } from '@tanstack/react-router';
import type { FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { AggregateCard } from '../AggregateCard/AggregateCard';
import type { Responder } from '../../models/form-aggregated';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';

export default function FormAggregatedDetails(): FunctionComponent {
  const formSubmission = useLoaderData({ from: '/responses/$formId/aggregated' });
  const { submissionsAggregate } = formSubmission;
  const { defaultLanguage, formCode, formType, aggregates, formId, responders } = submissionsAggregate;

  const groupedAttachments = responders.reduce<Record<string, Responder>>(
    (grouped, responder) => ({
      ...grouped,
      [responder.responderId]: responder,
    }),
    {}
  );

  return (
    <Layout
      backButton={<NavigateBack to='/responses' />}
      breadcrumbs={
        <div className='breadcrumbs flex flex-row gap-2 mb-4'>
          <Link className='crumb' to='/responses' preload='intent'>
            responses
          </Link>
          <Link className='crumb'>{formId}</Link>
        </div>
      }
      title={`${formCode} - ${formType.name}`}>
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
