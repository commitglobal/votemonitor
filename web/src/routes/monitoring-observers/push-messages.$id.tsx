import type { FunctionComponent } from '@/common/types';
import { createFileRoute, useLoaderData, useNavigate } from '@tanstack/react-router';
import { pushMessageDetailsQueryOptions } from './push-messages.$id_.view';

function Details(): FunctionComponent {
  const navigate = useNavigate({ from: '/monitoring-observers/push-messages/$id' });
  const pushMessageData = useLoaderData({ from: '/monitoring-observers/push-messages/$id' });

  if (pushMessageData.id) {
    console.log('navigate', pushMessageData);

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
  loader: ({ context: { queryClient }, params: { id } }) =>
    queryClient.ensureQueryData(pushMessageDetailsQueryOptions(id)),
});
