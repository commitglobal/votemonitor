import { authApi } from '@/common/auth-api';
import EditMonitoringObserver from '@/features/monitoring-observers/components/EditMonitoringObserver/EditMonitoringObserver';
import { monitoringObserversKeys } from '@/features/monitoring-observers/hooks/monitoring-observers-queries';
import { MonitoringObserver } from '@/features/monitoring-observers/models/monitoring-observer';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export const monitoringObserverDetailsQueryOptions = (electionRoundId: string, monitoringObserverId: string) => {
  return queryOptions({
    queryKey: monitoringObserversKeys.detail(electionRoundId, monitoringObserverId),
    queryFn: async () => {
      const response = await authApi.get<MonitoringObserver>(
        `/election-rounds/${electionRoundId}/monitoring-observers/${monitoringObserverId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch monitoring observer details');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
  });
};

export const Route = createFileRoute('/monitoring-observers/edit/$monitoringObserverId')({
  component: Edit,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { monitoringObserverId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(monitoringObserverDetailsQueryOptions(electionRoundId, monitoringObserverId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});

function Edit() {
  return (
    <div className='p-2'>
      <EditMonitoringObserver />
    </div>
  );
}
