import { FormBuilderScreenReuse } from '@/features/election-event/components/FormBuilder/components/FormBuilderScreenReuse';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/new/reuse')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewFormFromOldForm,
});

function CreateNewFormFromOldForm() {
  return <FormBuilderScreenReuse />;
}
