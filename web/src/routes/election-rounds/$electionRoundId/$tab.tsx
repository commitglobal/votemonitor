import ElectionRoundDetails from '@/features/election-rounds/components/ElectionRoundDetails/ElectionRoundDetails';
import { electionRoundDetailsQueryOptions } from '@/features/election-rounds/queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/election-rounds/$electionRoundId/$tab')({
  component: ElectionRoundDetails,
  loader: ({ context: { queryClient }, params: { electionRoundId } }) =>
    queryClient.ensureQueryData(electionRoundDetailsQueryOptions(electionRoundId)),
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
});
