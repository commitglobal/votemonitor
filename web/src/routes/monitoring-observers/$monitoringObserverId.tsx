import { authApi } from '@/common/auth-api';
import MonitoringObserverDetails from '@/features/monitoring-observers/components/MonitoringObserverDetails/MonitoringObserverDetails';
import { MonitoringObserver } from '@/features/monitoring-observers/models/MonitoringObserver';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export const monitoringObserverQueryOptions = (monitoringObserverId: string) =>
  queryOptions({
    queryKey: ['monitoring-observer', { monitoringObserverId }],
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');
      const monitoringNgoId: string | null = localStorage.getItem('monitoringNgoId');

      const response = await authApi.get<MonitoringObserver>(
        `/election-rounds/${electionRoundId}/monitoring-ngos/${monitoringNgoId}/monitoring-observers/${monitoringObserverId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngo');
      }

      return response.data;
    },
  });

export const Route = createFileRoute('/monitoring-observers/$monitoringObserverId')({
  beforeLoad: ({ context }) => {
    redirectIfNotAuth(context.authContext.isAuthenticated);
  },
  component: Details,
  loader: ({ context: { queryClient }, params: { monitoringObserverId } }) =>
    queryClient.ensureQueryData(monitoringObserverQueryOptions(monitoringObserverId)),
});

function Details() {
  return (
    <div className='p-2'>
      <MonitoringObserverDetails />
    </div>
  );
}
