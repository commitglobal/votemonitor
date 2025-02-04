import { authApi } from '@/common/auth-api';
import { DataTableParameters, ElectionRoundStatus, PageResponse } from '@/common/types';
import { UseQueryResult, queryOptions, useQuery } from '@tanstack/react-query';
import { ElectionRoundModel } from './models/types';
import { buildURLSearchParams } from '@/lib/utils';

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
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<PageResponse<ElectionRoundModel>>(`/election-rounds`, {
        params: searchParams,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch election rounds');
      }

      return response.data;
    },
  });
}

export const electionRoundDetailsQueryOptions = (electionRoundId: string) => {
  return queryOptions({
    queryKey: electionRoundKeys.detail(electionRoundId),
    queryFn: async () => {
      const response = await authApi.get<ElectionRoundModel>(`/election-rounds/${electionRoundId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch election round');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
  });
};
