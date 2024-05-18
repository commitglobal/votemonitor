import PushMessageForm from '@/features/monitoring-observers/components/PushMessageForm/PushMessageForm';
import { PushMessageTargetedObserversSearchParamsSchema } from '@/features/monitoring-observers/models/search-params';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/monitoring-observers/create-new-message')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewPushMessage,
  validateSearch: PushMessageTargetedObserversSearchParamsSchema,
});

function CreateNewPushMessage() {
  return (
    <PushMessageForm />
  );
}
