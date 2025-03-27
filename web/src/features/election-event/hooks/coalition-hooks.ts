import { authApi } from '@/common/auth-api';
import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { Coalition } from './../../../common/types';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const coalitionKeys = {
  all: (electionRoundId: string) => ['coalitions', electionRoundId] as const,
};

type CoalitionDetailResult = UseQueryResult<Coalition, Error>;

export function useCoalitionDetails(electionRoundId: string): CoalitionDetailResult {
  return useQuery({
    queryKey: coalitionKeys.all(electionRoundId!),
    queryFn: async () => {
      const response = await authApi.get<Coalition>(`/election-rounds/${electionRoundId}/coalitions:my`);

      return {
        ...response.data,
      };
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
}
