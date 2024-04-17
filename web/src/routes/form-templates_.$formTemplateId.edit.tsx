import { createFileRoute } from '@tanstack/react-router';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import EditFormTemplate from '@/features/form-templates/components/EditFormTemplate/EditFormTemplate';

export const Route = createFileRoute('/form-templates/$formTemplateId/edit')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { formTemplateId } }) =>
    queryClient.ensureQueryData(formTemplateDetailsQueryOptions(formTemplateId)),
});

function Edit() {
  return (
    <div className='p-2'>
      <EditFormTemplate />
    </div>
  );
}

