import FormDetails from '@/features/forms/components/FormDetails/FormDetails';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { createFileRoute } from '@tanstack/react-router';


export const Route = createFileRoute('/forms/$formId/$languageCode')({
  component: Details,
  loader: ({ context: { queryClient }, params: { formId } }) =>
    queryClient.ensureQueryData(formDetailsQueryOptions(formId)),
});


function Details() {
  return (
    <div className='p-2'>
      <FormDetails />
    </div>
  );
}