import { FormBuilderScreenScratch } from '@/features/election-event/components/FormBuilder/components/FormBuilderScreenScratch';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/new/scratch')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewFormFromScratch,
});

function CreateNewFormFromScratch() {
  return <FormBuilderScreenScratch />;
}
