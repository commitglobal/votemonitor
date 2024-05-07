import { useLoaderData } from '@tanstack/react-router';
import type { FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { AggregateCard } from '../AggregateCard/AggregateCard';

export default function FormAggregatedDetails(): FunctionComponent {
  const formSubmission = useLoaderData({ from: '/responses/$formId/aggregated' });
  const { submissionsAggregate } = formSubmission;
  const { defaultLanguage, formCode, formType, aggregates } = submissionsAggregate;

  return (
    <Layout title={`${formCode} - ${formType.name}`}>
      <div className='flex flex-col gap-10'>
        {Object.values(aggregates).map((aggregate) => {
          return <AggregateCard key={aggregate.questionId} aggregate={aggregate} language={defaultLanguage} />;
        })}
      </div>
    </Layout>
  );
}
