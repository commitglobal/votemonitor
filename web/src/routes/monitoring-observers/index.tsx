import MonitoringObserversDashboard from '@/features/monitoring-observers/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/monitoring-observers/')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: MonitoringObservers,
});

function MonitoringObservers() {
  return (
    <div className='p-2'>
      <MonitoringObserversDashboard />
    </div>
  );
}
