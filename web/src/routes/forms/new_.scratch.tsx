import { CreateFormPage } from '@/components/CreateFormPage/CreateFormPage';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/new_/scratch')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewFormFromScratch,
});

function CreateNewFormFromScratch() {
  return <CreateFormPage />;
}
