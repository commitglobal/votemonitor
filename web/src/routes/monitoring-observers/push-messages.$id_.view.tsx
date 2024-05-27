import { authApi } from '@/common/auth-api';
import PushMessageDetails from '@/features/monitoring-observers/components/PushMessageDetails/PushMessageDetails';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

import type { FunctionComponent } from '@/common/types';
import type { PushMessageDetailedModel } from '@/features/monitoring-observers/models/push-message';
import { pushMessagesKeys } from '@/features/monitoring-observers/hooks/push-messages-queries';

export const pushMessageDetailsQueryOptions = (pushMessageId: string) =>
  queryOptions({
    queryKey: pushMessagesKeys.detail(pushMessageId),
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<PushMessageDetailedModel>(
        `/election-rounds/${electionRoundId}/notifications/${pushMessageId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch notification details');
      }

      return response.data;
    },
  });

function Details(): FunctionComponent {
  return <PushMessageDetails />;
}

export const Route = createFileRoute('/monitoring-observers/push-messages/$id/view')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Details,
  loader: ({ context: { queryClient }, params: { id } }) =>
    queryClient.ensureQueryData(pushMessageDetailsQueryOptions(id)),
});
