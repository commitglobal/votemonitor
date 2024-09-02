import EditForm from '@/features/forms/components/EditForm/EditForm';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/$formId/edit')({
  component: Edit,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formDetailsQueryOptions(electionRoundId, formId))
  },
});

function Edit() {
  return (
    <div className='p-2 flex flex-col flex-1'>
      <EditForm />
    </div>
  );
}
