import FormEdit from '@/features/forms/components/FormEdit/FormEdit';
import FormNew from '@/features/forms/components/FormNew/FormNew';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/forms/new_/scratch')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: ()=><FormNew />,
});

