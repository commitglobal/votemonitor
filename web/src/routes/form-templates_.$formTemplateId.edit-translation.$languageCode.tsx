import { createFileRoute } from '@tanstack/react-router';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import EditFormTemplateTranslation from '@/features/form-templates/components/EditFormTemplateTranslation/EditFormTemplateTranslation';

export const Route = createFileRoute('/form-templates/$formTemplateId/edit-translation/$languageCode')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { formTemplateId } }) =>
    queryClient.ensureQueryData(formTemplateDetailsQueryOptions(formTemplateId)),
  
});

function Edit() {
  return (
    <div className='p-2'>
      <EditFormTemplateTranslation />
    </div>
  );
}

