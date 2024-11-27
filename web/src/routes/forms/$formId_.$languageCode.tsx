import PreviewForm from '@/features/forms/components/PreviewForm/PreviewForm';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/$formId/$languageCode')({
  component: Details,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formDetailsQueryOptions(electionRoundId, formId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});

function Details() {
  return (
    <div className='p-2'>
      <PreviewForm />
    </div>
  );
}
