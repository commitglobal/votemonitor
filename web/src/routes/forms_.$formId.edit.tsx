import EditForm from '@/features/forms/components/EditForm/EditForm';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

import { z } from 'zod';

const editFormParamsSchema = z.object({
  tab: z.enum(['form-details', 'questions']).catch('form-details').optional(),
});

export const Route = createFileRoute('/forms/$formId/edit')({
  component: Edit,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formDetailsQueryOptions(electionRoundId, formId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  validateSearch: (search) => editFormParamsSchema.parse(search),
});

function Edit() {
  const { tab } = Route.useSearch();
  return (
    <div className='p-2 flex flex-col flex-1'>
      <EditForm currentTab={tab} />
    </div>
  );
}
