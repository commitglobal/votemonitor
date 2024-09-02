import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { CitizenReport } from '../models/citizen-report';

const STALE_TIME = 1000 * 60; // one minute

export const citizenReportKeys = {
  all: (electionRoundId: string) => ['citizen-reports', electionRoundId] as const,
  lists: (electionRoundId: string) => [...citizenReportKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) => [...citizenReportKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...citizenReportKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...citizenReportKeys.details(electionRoundId), id] as const,
}

type CitizenReportsResponse = PageResponse<CitizenReport>;

type UseCitizenReportsResult = UseQueryResult<CitizenReportsResponse, Error>;

export function useCitizenReports(electionRoundId: string, queryParams: DataTableParameters): UseCitizenReportsResult {
  return useQuery({
    queryKey: citizenReportKeys.list(electionRoundId, queryParams),
    queryFn: async () => {

      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };

      const searchParams = new URLSearchParams(params);

      const response = await authApi.get<CitizenReportsResponse>(`/election-rounds/${electionRoundId}/citizen-reports`, {
        params: searchParams,
      });

      return response.data;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId
  });
}
