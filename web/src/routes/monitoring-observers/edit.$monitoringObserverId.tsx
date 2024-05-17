import { createFileRoute } from '@tanstack/react-router';
import EditMonitoringObserver from '@/features/monitoring-observers/components/EditMonitoringObserver/EditMonitoringObserver';
import { monitoringObserverQueryOptions } from './$monitoringObserverId.view.$tab';

export const Route = createFileRoute('/monitoring-observers/edit/$monitoringObserverId')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { monitoringObserverId } }) =>
    queryClient.ensureQueryData(monitoringObserverQueryOptions(monitoringObserverId)),
});

function Edit() {
  return (
    <div className='p-2'>
      <EditMonitoringObserver />
    </div>
  );
}
