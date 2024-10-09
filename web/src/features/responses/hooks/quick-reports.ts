import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { QuickReport, QuickReportsFilters } from '../models/quick-report';

const STALE_TIME = 1000 * 60; // one minute

export const quickReportKeys = {
  all: (electionRoundId: string) => ['quick-reports', electionRoundId] as const,
  lists: (electionRoundId: string) => [...quickReportKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) => [...quickReportKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...quickReportKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...quickReportKeys.details(electionRoundId), id] as const,
  filters: (electionRoundId: string) => [...quickReportKeys.details(electionRoundId), 'filters'] as const,
};

type QuickReportsResponse = PageResponse<QuickReport>;

type UseQuickReportsResult = UseQueryResult<QuickReportsResponse, Error>;

export function useQuickReports(electionRoundId: string, queryParams: DataTableParameters): UseQuickReportsResult {
  return useQuery({
    queryKey: quickReportKeys.list(electionRoundId, queryParams),
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



export function useQuickReportsFilters(electionRoundId: string) {
  return useQuery({
    queryKey: quickReportKeys.filters(electionRoundId),
    queryFn: async () => {
      const response = await authApi.get<QuickReportsFilters>(
        `/election-rounds/${electionRoundId}/quick-reports:filters`
      );

      return response.data;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}