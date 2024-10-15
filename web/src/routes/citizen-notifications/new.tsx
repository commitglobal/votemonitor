import CitizenNotificationMessageForm from '@/features/CitizenNotifications/CitizenNotificationMessageForm/CitizenNotificationMessageForm';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/citizen-notifications/new')({
  component: CitizenNotificationMessageForm,
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});
