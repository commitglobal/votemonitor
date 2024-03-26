
import { createFileRoute } from '@tanstack/react-router';
import { authApi } from '@/common/auth-api';
import { ElectionRound } from '@/features/election-round/models/ElectionRound';
import { queryOptions } from '@tanstack/react-query';


export const electionRoundQueryOptions = (electionRoundId: string) =>
  queryOptions({
    queryKey: ['election-rounds', { electionRoundId }],
    queryFn: async () => {
      const response = await authApi.get<ElectionRound>(`/election-rounds/${electionRoundId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch election round');
      }

      return response.data;
    },
  });

export const Route = createFileRoute('/election-rounds/$electionRoundId')({
  component: ElectionRoundDetails,
  loader: ({ context: { queryClient }, params: { electionRoundId } }) => queryClient.ensureQueryData(electionRoundQueryOptions(electionRoundId))
});

function ElectionRoundDetails() {
  const ngo = Route.useLoaderData();

  return <div className="p-2">
    Hello from election round!
    <pre> {JSON.stringify(ngo, null, 2)}</pre>
  </div>
}