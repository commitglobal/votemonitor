import { FormBuilder } from '@/features/election-event/components/FormBuilder/components/FormBuilder';
import { PushMessageTargetedObserversSearchParamsSchema } from '@/features/monitoring-observers/models/search-params';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/election-event/new-form')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewForm,
  validateSearch: PushMessageTargetedObserversSearchParamsSchema,
});

function CreateNewForm() {
  return <FormBuilder />;
}
