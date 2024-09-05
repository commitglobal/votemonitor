import { monitoringObserverDetailsQueryOptions } from '@/common/queryOptions';
import EditMonitoringObserver from '@/features/monitoring-observers/components/EditMonitoringObserver/EditMonitoringObserver';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/monitoring-observers/edit/$monitoringObserverId')({
  component: Edit,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { monitoringObserverId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(monitoringObserverDetailsQueryOptions(electionRoundId, monitoringObserverId))
  },
});

function Edit() {
  return (
    <div className='p-2'>
      <EditMonitoringObserver />
    </div>
  );
}
