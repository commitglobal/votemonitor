import { getCoalitionDetails } from '@/api/election-event/get-coalition-details';
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
      return getCoalitionDetails(electionRoundId);
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
}
