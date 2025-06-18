import FormTemplateEdit from '@/features/form-templates/components/FormTemplateEdit/FormTemplateEdit';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/form-templates/$formTemplateId_/edit')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { formTemplateId } }) => {
    return queryClient.ensureQueryData(formTemplateDetailsQueryOptions(formTemplateId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
});

function Edit() {
  return <FormTemplateEdit />;
}
