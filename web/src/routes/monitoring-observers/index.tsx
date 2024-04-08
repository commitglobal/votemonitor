import MonitoringObserversDashboard from '@/features/monitoring-observers/components/Dashboard/Dashboard';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/monitoring-observers/')({
  component: Observers,
});

function Observers() {
  return (
    <div className='p-2'>
      <MonitoringObserversDashboard />
    </div>
  );
}
