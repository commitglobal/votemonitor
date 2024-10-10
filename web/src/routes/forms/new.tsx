import { FormBuilderScreenStart } from '@/features/election-event/components/FormBuilder/components/FormBuilderScreenStart';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/new')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewForm,
});

function CreateNewForm() {
  return <FormBuilderScreenStart />;
}
