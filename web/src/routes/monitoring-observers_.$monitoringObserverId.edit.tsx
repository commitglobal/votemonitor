import { createFileRoute } from '@tanstack/react-router';
import EditMonitoringObserver from '@/features/monitoring-observers/components/EditMonitoringObserver/EditMonitoringObserver';
import { monitoringObserverQueryOptions } from './monitoring-observers/$monitoringObserverId';

export const Route = createFileRoute('/monitoring-observers/$monitoringObserverId/edit')({
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
