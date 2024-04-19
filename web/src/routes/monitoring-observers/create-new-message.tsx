import PushMessageForm from '@/features/monitoring-observers/components/PushMessageForm/PushMessageForm';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/monitoring-observers/create-new-message')({
  beforeLoad: ({ context }) => {
    redirectIfNotAuth(context.authContext.isAuthenticated);
  },
  component: CreateNewPushMessage,
});

function CreateNewPushMessage() {
  return (
    <div className='p-2'>
      <PushMessageForm />
    </div>
  );
}
