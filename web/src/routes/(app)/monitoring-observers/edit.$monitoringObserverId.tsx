import API from '@/services/api';
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
      const response = await API.get<MonitoringObserver>(
        `/election-rounds/${electionRoundId}/monitoring-observers/${monitoringObserverId}`
      );

      return response.data;
    },
    enabled: !!electionRoundId,
  });
};

export const Route = createFileRoute('/(app)/monitoring-observers/edit/$monitoringObserverId')({
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
  return <EditMonitoringObserver />;
}
