import { FormBuilderScreenStart } from '@/features/forms/components/FormBuilder/components/FormBuilderScreenStart';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/forms/new')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewForm,
});

function CreateNewForm() {
  return <FormBuilderScreenStart />;
}
