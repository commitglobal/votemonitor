import FormDetails from '@/features/forms/components/FormDetails/FormDetails';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { createFileRoute } from '@tanstack/react-router';


export const Route = createFileRoute('/forms/$formId/$languageCode')({
  component: Details,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formDetailsQueryOptions(electionRoundId, formId));
  }
});

function Details() {
  return (
    <div className='p-2'>
      <FormDetails />
    </div>
  );
}