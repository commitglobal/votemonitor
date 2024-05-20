import { type EnsureQueryDataOptions, queryOptions, type QueryKey } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';
import { authApi } from '@/common/auth-api';
import type { FunctionComponent } from '@/common/types';
import MonitoringObserverDetails from '@/features/monitoring-observers/components/MonitoringObserverDetails/MonitoringObserverDetails';
import {
  type MonitoringObserver,
  monitoringObserverDetailsRouteSearchSchema,
} from '@/features/monitoring-observers/models/monitoring-observer';
import { redirectIfNotAuth } from '@/lib/utils';

export const monitoringObserverQueryOptions = (
  monitoringObserverId: string
): EnsureQueryDataOptions<MonitoringObserver> =>
  queryOptions({
    queryKey: ['monitoring-observer', { monitoringObserverId }] as QueryKey,
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<MonitoringObserver>(
        `/election-rounds/${electionRoundId}/monitoring-observers/${monitoringObserverId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch monitoring observer details');
      }

      return response.data;
    },
  });

export const Route = createFileRoute('/monitoring-observers/view/$monitoringObserverId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Details,
  loader: ({ context: { queryClient }, params: { monitoringObserverId } }) =>
    queryClient.ensureQueryData(monitoringObserverQueryOptions(monitoringObserverId)),
  validateSearch: monitoringObserverDetailsRouteSearchSchema
});

function Details(): FunctionComponent {
  return (
    <div className='p-2'>
      <MonitoringObserverDetails />
    </div>
  );
}