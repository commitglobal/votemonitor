import FormEditor from '@/components/FormEditor/FormEditor';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forms/new_/scratch')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: FormEditor,
});

