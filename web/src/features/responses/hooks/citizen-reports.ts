import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { RowData } from '@/components/ui/DataTable/DataTable';
import { buildURLSearchParams } from '@/lib/utils';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';

import type { CitizenReportByEntry } from '../models/citizen-report';

const STALE_TIME = 1000 * 60; // one minute

export const citizenReportKeys = {
  all: (electionRoundId: string) => ['citizen-reports', electionRoundId] as const,
  lists: (electionRoundId: string) => [...citizenReportKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) => [...citizenReportKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...citizenReportKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...citizenReportKeys.details(electionRoundId), id] as const,
}

type CitizenReportsByEntryResponse = PageResponse<CitizenReportByEntry & RowData>;

type UseCitizenReportsByEntryResult = UseQueryResult<CitizenReportsByEntryResponse, Error>;

export function useCitizenReports(electionRoundId: string, queryParams: DataTableParameters): UseCitizenReportsByEntryResult {
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
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<CitizenReportsByEntryResponse>(
        `/election-rounds/${electionRoundId}/citizen-reports:byEntry`,
        {
          params: searchParams,
        }
      );

      return {
        ...response.data,
        items: response.data.items.map((submission) => ({ ...submission, id: submission.citizenReportId })),
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId
  });
}
