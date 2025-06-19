import API from '@/services/api';
import PushMessageDetails from '@/features/monitoring-observers/components/PushMessageDetails/PushMessageDetails';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

import type { FunctionComponent } from '@/common/types';
import { pushMessagesKeys } from '@/features/monitoring-observers/hooks/push-messages-queries';
import type { PushMessageDetailedModel } from '@/features/monitoring-observers/models/push-message';

export const pushMessageDetailsQueryOptions = (electionRoundId: string, pushMessageId: string) => {
  return queryOptions({
    queryKey: pushMessagesKeys.detail(electionRoundId, pushMessageId),
    queryFn: async () => {
      const response = await API.get<PushMessageDetailedModel>(
        `/election-rounds/${electionRoundId}/notifications/${pushMessageId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch notification details');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
  });
};

function Details(): FunctionComponent {
  return <PushMessageDetails />;
}

export const Route = createFileRoute('/(app)/monitoring-observers/push-messages/$id_/view')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Details,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { id } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(pushMessageDetailsQueryOptions(electionRoundId, id));
  },
});
