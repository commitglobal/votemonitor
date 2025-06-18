import FormTemplateTranslationEdit from '@/features/form-templates/components/FormTemplateTranslationEdit/FormTemplateTranslationEdit';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/form-templates/$formTemplateId_/edit-translation/$languageCode')({
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
  return (
      <FormTemplateTranslationEdit />
  );
}
