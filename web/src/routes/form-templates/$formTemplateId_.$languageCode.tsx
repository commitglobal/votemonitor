import FormTemplateDetails from '@/features/form-templates/components/FormTemplateDetails/FormTemplateDetails';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import { createFileRoute } from '@tanstack/react-router';


export const Route = createFileRoute('/form-templates/$formTemplateId/$languageCode')({
  component: Details,
  loader: ({ context: { queryClient }, params: { formTemplateId } }) =>
    queryClient.ensureQueryData(formTemplateDetailsQueryOptions(formTemplateId)),
});


function Details() {
  return (
    <div className='p-2'>
      <FormTemplateDetails />
    </div>
  );
}