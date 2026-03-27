import { getElectionRoundDetails } from '@/api/election-rounds/get-election-round-details';
import { getElectionRounds } from '@/api/election-rounds/get-election-rounds';
import { DataTableParameters, ElectionRoundStatus, PageResponse } from '@/common/types';
import { UseQueryResult, queryOptions, useQuery } from '@tanstack/react-query';
import { ElectionRoundModel } from './models/types';
const STALE_TIME = 1000 * 60 * 15; // fifteen minutes

export const electionRoundKeys = {
  all: ['electionRounds'] as const,
  lists: () => [...electionRoundKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...electionRoundKeys.lists(), { ...params }] as const,
  details: () => [...electionRoundKeys.all, 'detail'] as const,
  detail: (id: string) => [...electionRoundKeys.details(), id] as const,
};

export interface ElectionsRoundsQueryParams {
  searchText: string | undefined;
  countryId: string | undefined;
  electionRoundStatus: ElectionRoundStatus | undefined;
}

export function useElectionRounds(
  queryParams: DataTableParameters
): UseQueryResult<PageResponse<ElectionRoundModel>, Error> {
  return useQuery({
    queryKey: electionRoundKeys.list(queryParams),
    queryFn: async () => {
      return getElectionRounds(queryParams);
    },
    staleTime: STALE_TIME,
  });
}

export const electionRoundDetailsQueryOptions = (electionRoundId: string) => {
  return queryOptions({
    queryKey: electionRoundKeys.detail(electionRoundId),
    queryFn: async () => {
      return getElectionRoundDetails(electionRoundId);
    },
    enabled: !!electionRoundId,
  });
};
