import FormTemplateTranslationEdit from '@/features/form-templates/components/FormTemplateTranslationEdit/FormTemplateTranslationEdit';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/form-templates/$formTemplateId_/edit-translation/$languageCode')({
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
    <div className='p-2 flex-1'>
      <FormTemplateTranslationEdit />
    </div>
  );
}
