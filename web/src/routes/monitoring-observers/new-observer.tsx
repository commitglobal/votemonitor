import CreateMonitoringObserver from '@/features/monitoring-observers/components/CreateMonitoringObserver';
import CreateMonitoringObserverForm from '@/features/monitoring-observers/components/CreateMonitoringObserverForm';
import PushMessageForm from '@/features/monitoring-observers/components/PushMessageForm/PushMessageForm';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/monitoring-observers/new-observer')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CreateNewObserver,
});

function CreateNewObserver() {
  return <CreateMonitoringObserver />;
}
