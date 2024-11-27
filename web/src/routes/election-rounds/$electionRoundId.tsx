
import { authApi } from '@/common/auth-api';
import { ElectionRound } from '@/features/election-round/models/ElectionRound';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions, useSuspenseQuery } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export const electionRoundQueryOptions = (electionRoundId: string) => {
  return queryOptions({
    queryKey: ['election-rounds', { electionRoundId }],
    queryFn: async () => {

      const response = await authApi.get<ElectionRound>(`/election-rounds/${electionRoundId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch election round');
      }

      return response.data;
    },
    enabled: !!electionRoundId
  });
}

export const Route = createFileRoute('/election-rounds/$electionRoundId')({
  component: ElectionRoundDetails,
  loader: ({ context: { queryClient }, params: { electionRoundId } }) => queryClient.ensureQueryData(electionRoundQueryOptions(electionRoundId)),
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});

function ElectionRoundDetails() {
  const { electionRoundId } = Route.useParams();
  const { data: electionRound } = useSuspenseQuery(electionRoundQueryOptions(electionRoundId));

  return <div className="p-2">
    Hello from election round!
    <pre> {JSON.stringify(electionRound, null, 2)}</pre>
  </div>
}