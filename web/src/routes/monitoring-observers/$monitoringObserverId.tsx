import { type EnsureQueryDataOptions, queryOptions, type QueryKey } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';
import { authApi } from '@/common/auth-api';
import type { FunctionComponent } from '@/common/types';
import MonitoringObserverDetails from '@/features/monitoring-observers/components/MonitoringObserverDetails/MonitoringObserverDetails';
import {
  type MonitoringObserver,
  monitoringObserverDetailsRouteSearchSchema,
} from '@/features/monitoring-observers/models/MonitoringObserver';
import { redirectIfNotAuth } from '@/lib/utils';

export const monitoringObserverQueryOptions = (
  monitoringObserverId: string
): EnsureQueryDataOptions<MonitoringObserver> =>
  queryOptions({
    queryKey: ['monitoring-observer', { monitoringObserverId }] as QueryKey,
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

function Details(): FunctionComponent {
  return (
    <div className='p-2'>
      <MonitoringObserverDetails />
    </div>
  );
}

export const Route = createFileRoute('/monitoring-observers/$monitoringObserverId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Details,
  loader: ({ context: { queryClient }, params: { monitoringObserverId } }) =>
    queryClient.ensureQueryData(monitoringObserverQueryOptions(monitoringObserverId)),
  validateSearch: monitoringObserverDetailsRouteSearchSchema,
});
