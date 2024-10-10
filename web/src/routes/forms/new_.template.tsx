import { FormBuilderScreenTemplate } from '@/features/election-event/components/FormBuilder/components/FormBuilderScreenTemplate';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/new/template')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewFormFromTemplate,
});

function CreateNewFormFromTemplate() {
  return <FormBuilderScreenTemplate />;
}
