import { createFileRoute } from '@tanstack/react-router';
import EditMonitoringObserver from '@/features/monitoring-observers/components/EditMonitoringObserver/EditMonitoringObserver';
import { monitoringObserverDetailsQueryOptions } from '@/common/queryOptions';

export const Route = createFileRoute('/monitoring-observers/edit/$monitoringObserverId')({
  component: Edit,
  loader: ({ context: { queryClient }, params: { monitoringObserverId } }) =>
    queryClient.ensureQueryData(monitoringObserverDetailsQueryOptions(monitoringObserverId)),
});

function Edit() {
  return (
    <div className='p-2'>
      <EditMonitoringObserver />
    </div>
  );
}
