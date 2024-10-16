import CreateMonitoringObserver from '@/features/monitoring-observers/components/CreateMonitoringObserver';
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
