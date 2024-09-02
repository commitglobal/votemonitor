import EditFormTranslation from '@/features/forms/components/EditFormTranslation/EditFormTranslation';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/$formId/edit-translation/$languageCode')({
  component: Edit,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) =>{
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formDetailsQueryOptions(electionRoundId, formId));}
});

function Edit() {
  return (
    <div className='p-2 flex-1'>
      <EditFormTranslation />
    </div>
  );
}
