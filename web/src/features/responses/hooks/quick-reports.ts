import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { QuickReport } from '../models/quick-report';

const STALE_TIME = 1000 * 60; // one minute

export const quickReportKeys = {
  all: ['quick-reports'] as const,
  lists: () => [...quickReportKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...quickReportKeys.lists(), { ...params }] as const,
  details: () => [...quickReportKeys.all, 'detail'] as const,
  detail: (id: string) => [...quickReportKeys.details(), id] as const,
}

type QuickReportsResponse = PageResponse<QuickReport>;

type UseQuickReportsResult = UseQueryResult<QuickReportsResponse, Error>;

export function useQuickReports(electionRoundId: string, queryParams: DataTableParameters): UseQuickReportsResult {
  return useQuery({
    queryKey: quickReportKeys.list(queryParams),
    queryFn: async () => {

      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };

      const searchParams = new URLSearchParams(params);

      const response = await authApi.get<QuickReportsResponse>(`/election-rounds/${electionRoundId}/quick-reports`, {
        params: searchParams,
      });

      return response.data;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId
  });
}
