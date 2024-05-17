import { authApi } from '@/common/auth-api';
import PushMessageDetails from '@/features/monitoring-observers/components/PushMessageDetails/PushMessageDetails';
import { MonitoringObserver } from '@/features/monitoring-observers/models/MonitoringObserver';
import { redirectIfNotAuth } from '@/lib/utils';
import { EnsureQueryDataOptions, QueryKey, queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

import type { FunctionComponent } from '@/common/types';
export const pushMessageDetailsQueryOptions = (
  pushMessageId: string
): EnsureQueryDataOptions<MonitoringObserver> =>
  queryOptions({
    queryKey: ['monitoring-observer', { monitoringObserverId: pushMessageId }] as QueryKey,
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<MonitoringObserver>(
        `/election-rounds/${electionRoundId}/notifications/${pushMessageId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngo');
      }

      return response.data;
    },
  });

function Details(): FunctionComponent {
  return (
      <PushMessageDetails />
  );
}

export const Route = createFileRoute('/monitoring-observers/push-messages/$id/view')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Details,
  loader: ({ context: { queryClient }, params: { id } }) =>
    queryClient.ensureQueryData(pushMessageDetailsQueryOptions(id)),
});
