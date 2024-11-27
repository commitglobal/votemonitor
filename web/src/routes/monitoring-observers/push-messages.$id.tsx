import type { FunctionComponent } from '@/common/types';
import { createFileRoute, useLoaderData, useNavigate } from '@tanstack/react-router';
import { pushMessageDetailsQueryOptions } from './push-messages.$id_.view';
import { redirectIfNotAuth } from '@/lib/utils';

function Details(): FunctionComponent {
  const navigate = useNavigate({ from: '/monitoring-observers/push-messages/$id' });
  const pushMessageData = useLoaderData({ from: '/monitoring-observers/push-messages/$id' });

  if (pushMessageData.id) {
    void navigate({
      to: '/monitoring-observers/push-messages/$id/view',
      params: { id: pushMessageData.id },
      replace: true,
    });
  }

  return null;
}

export const Route = createFileRoute('/monitoring-observers/push-messages/$id')({
  component: Details,
  loader: ({ context: { queryClient , currentElectionRoundContext}, params: { id } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(pushMessageDetailsQueryOptions(electionRoundId, id));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});
