import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import { UseQueryResult, queryOptions, useQuery } from '@tanstack/react-query';
import { ElectionRoundModel } from './models/types';

export const electionRoundKeys = {
  all: ['electionRounds'] as const,
  lists: () => [...electionRoundKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...electionRoundKeys.lists(), { ...params }] as const,
  details: () => [...electionRoundKeys.all, 'detail'] as const,
  detail: (id: string) => [...electionRoundKeys.details(), id] as const,
};

export function useElectionRounds(params: DataTableParameters): UseQueryResult<PageResponse<ElectionRoundModel>, Error> {
  return useQuery({
    queryKey: electionRoundKeys.list(params),
    queryFn: async () => {
      const response = await authApi.get<PageResponse<ElectionRoundModel>>('/election-rounds', {
        params: {
          PageNumber: params.pageNumber,
          PageSize: params.pageSize,
          SortColumnName: params.sortColumnName,
          SortOrder: params.sortOrder,
        },
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch electionRounds');
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
