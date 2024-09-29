import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { RowData } from '@/components/ui/DataTable/DataTable';
import { buildURLSearchParams } from '@/lib/utils';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import type { IssueReportByEntry, IssueReportByForm, IssueReportByObserver } from '../models/issue-report';

const STALE_TIME = 1000 * 60; // one minute

export const issueReportsByEntryKeys = {
  all: (electionRoundId: string) => ['issue-reports-by-entry', electionRoundId] as const,
  lists: (electionRoundId: string) => [...issueReportsByEntryKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...issueReportsByEntryKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...issueReportsByEntryKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...issueReportsByEntryKeys.details(electionRoundId), id] as const,
};

export const issueReportsByObserverKeys = {
  all: (electionRoundId: string) => ['issue-reports-by-observer', electionRoundId] as const,
  lists: (electionRoundId: string) => [...issueReportsByObserverKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...issueReportsByObserverKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...issueReportsByObserverKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...issueReportsByObserverKeys.details(electionRoundId), id] as const,
};

export const issueReportsAggregatedKeys = {
  all: (electionRoundId: string) => ['aggregated-issue-reports', electionRoundId] as const,
  lists: (electionRoundId: string) => [...issueReportsAggregatedKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...issueReportsAggregatedKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...issueReportsAggregatedKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...issueReportsAggregatedKeys.details(electionRoundId), id] as const,
};

type IssueReportsByEntryResponse = PageResponse<IssueReportByEntry & RowData>;

type UseIssueReportsByEntryResult = UseQueryResult<IssueReportsByEntryResponse, Error>;

export function useIssueReportsByEntry(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseIssueReportsByEntryResult {
  return useQuery({
    queryKey: issueReportsByEntryKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<IssueReportsByEntryResponse>(
        `/election-rounds/${electionRoundId}/issue-reports:byEntry`,
        {
          params: searchParams,
        }
      );

      return {
        ...response.data,
        items: response.data.items.map((submission) => ({ ...submission, id: submission.issueReportId })),
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

type IssueReportsByObserverResponse = PageResponse<IssueReportByObserver & RowData>;

type UseIssueReportsByObserverResult = UseQueryResult<IssueReportsByObserverResponse, Error>;

export function useIssueReportsByObserver(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseIssueReportsByObserverResult {
  return useQuery({
    queryKey: issueReportsByObserverKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<IssueReportsByObserverResponse>(
        `/election-rounds/${electionRoundId}/issue-reports:byObserver`,
        {
          params: searchParams,
        }
      );

      return {
        ...response.data,
        items: response.data.items.map((submission) => ({ ...submission, id: submission.monitoringObserverId })),
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

type IssueReportsByFormResponse = PageResponse<IssueReportByForm & RowData>;

type UseIssueReportsByFormResult = UseQueryResult<IssueReportsByFormResponse, Error>;

export function useIssueReportsByForm(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseIssueReportsByFormResult {
  return useQuery({
    queryKey: issueReportsAggregatedKeys.all(electionRoundId),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<{ aggregatedForms: IssueReportByForm[] }>(
        `/election-rounds/${electionRoundId}/issue-reports:byForm`,
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
