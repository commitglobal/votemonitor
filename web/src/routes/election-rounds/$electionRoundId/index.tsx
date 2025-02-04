import { electionRoundDetailsQueryOptions } from '@/features/election-rounds/queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, Navigate } from '@tanstack/react-router';

export const Route = createFileRoute('/election-rounds/$electionRoundId/')({
  component: Component,
  loader: ({ context: { queryClient }, params: { electionRoundId } }) =>
    queryClient.ensureQueryData(electionRoundDetailsQueryOptions(electionRoundId)),
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});

function Component() {
  const { electionRoundId } = Route.useParams();
  return <Navigate to='/election-rounds/$electionRoundId/$tab' params={{ electionRoundId, tab: 'event-details' }} />;
}
