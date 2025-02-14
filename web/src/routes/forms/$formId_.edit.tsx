import FormTemplateEdit from '@/features/form-templates/components/FormTemplateEdit/FormTemplateEdit';
import FormEdit from '@/features/forms/components/FormEdit/FormEdit';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/$formId_/edit')({
  component: Edit,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formDetailsQueryOptions(electionRoundId, formId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});

function Edit() {
  return <FormEdit />;
}
