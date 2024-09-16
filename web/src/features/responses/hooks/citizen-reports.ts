import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { RowData } from '@/components/ui/DataTable/DataTable';
import { buildURLSearchParams } from '@/lib/utils';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';

import type { CitizenReportByEntry, CitizenReportsAggregatedByForm } from '../models/citizen-report';

const STALE_TIME = 1000 * 60; // one minute

export const citizenReportKeys = {
  all: (electionRoundId: string) => ['citizen-reports', electionRoundId] as const,
  lists: (electionRoundId: string) => [...citizenReportKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) => [...citizenReportKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...citizenReportKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...citizenReportKeys.details(electionRoundId), id] as const,
}

export const citizenReportsAggregatedKeys = {
  all: (electionRoundId: string) => ['citizen-reports-aggregated', electionRoundId] as const,
  lists: (electionRoundId: string) => [...citizenReportsAggregatedKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...citizenReportsAggregatedKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...citizenReportsAggregatedKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...citizenReportsAggregatedKeys.details(electionRoundId), id] as const,
};

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


type CitizenReportsAggregatedByFormResponse = PageResponse<CitizenReportsAggregatedByForm & RowData>;

type UseCitizenReportsAggregatedByFormResult = UseQueryResult<CitizenReportsAggregatedByFormResponse, Error>;

export function useCitizenReportsAggregatedByForm(electionRoundId: string, queryParams: DataTableParameters): UseCitizenReportsAggregatedByFormResult {
  return useQuery({
    queryKey: citizenReportsAggregatedKeys.all(electionRoundId),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<{ aggregatedForms: CitizenReportsAggregatedByForm[] }>(
        `/election-rounds/${electionRoundId}/citizen-reports:byForm`,
        {
          params: searchParams,
        }
      );

      return {
        currentPage: 1,
        pageSize: queryParams.pageSize,
        totalCount: response.data.aggregatedForms.length,
        items: response.data.aggregatedForms.map((submission) => ({
          ...submission,
          id: submission.formId,
        })),
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

