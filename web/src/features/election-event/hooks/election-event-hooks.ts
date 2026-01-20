import { authApi } from '@/common/auth-api';
import { queryOptions, useQuery, UseQueryResult } from '@tanstack/react-query';

import { electionRoundKeys } from '@/features/election-rounds/queries';
import { ElectionEvent } from '../models/election-event';

const STALE_TIME = 1000 * 60 * 5; // five minutes

type ElectionEventResult = UseQueryResult<ElectionEvent, Error>;

export const electionRoundDetailsQueryOptions = (electionRoundId: string) => {
  return queryOptions({
    queryKey: electionRoundKeys.detail(electionRoundId!),
    queryFn: async () => {
      const response = await authApi.get<ElectionEvent>(`/election-rounds/${electionRoundId}`);

      return {
        ...response.data,
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
};

export function useElectionRoundDetails(electionRoundId: string): ElectionEventResult {
  return useQuery(electionRoundDetailsQueryOptions(electionRoundId));
}
