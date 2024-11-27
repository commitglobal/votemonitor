import { authApi } from '@/common/auth-api';
import type { DataSources, DataTableParameters, PageResponse } from '@/common/types';
import type { RowData } from '@/components/ui/DataTable/DataTable';
import { buildURLSearchParams } from '@/lib/utils';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import type { IncidentReportByEntry, IncidentReportByForm, IncidentReportByObserver, IncidentReportsFilters } from '../models/incident-report';

const STALE_TIME = 1000 * 60; // one minute

export const incidentReportsByEntryKeys = {
  all: (electionRoundId: string) => ['incident-reports-by-entry', electionRoundId] as const,
  lists: (electionRoundId: string) => [...incidentReportsByEntryKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...incidentReportsByEntryKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...incidentReportsByEntryKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...incidentReportsByEntryKeys.details(electionRoundId), id] as const,
  filters: (electionRoundId: string, dataSource: DataSources) =>
    [...incidentReportsByEntryKeys.all(electionRoundId), dataSource, 'filters'] as const,
};

export const incidentReportsByObserverKeys = {
  all: (electionRoundId: string) => ['incident-reports-by-observer', electionRoundId] as const,
  lists: (electionRoundId: string) => [...incidentReportsByObserverKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...incidentReportsByObserverKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...incidentReportsByObserverKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...incidentReportsByObserverKeys.details(electionRoundId), id] as const,
};

export const incidentReportsAggregatedKeys = {
  all: (electionRoundId: string) => ['aggregated-incident-reports', electionRoundId] as const,
  lists: (electionRoundId: string) => [...incidentReportsAggregatedKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...incidentReportsAggregatedKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...incidentReportsAggregatedKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...incidentReportsAggregatedKeys.details(electionRoundId), id] as const,
};

type IncidentReportsByEntryResponse = PageResponse<IncidentReportByEntry & RowData>;

type UseIncidentReportsByEntryResult = UseQueryResult<IncidentReportsByEntryResponse, Error>;

export function useIncidentReportsByEntry(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseIncidentReportsByEntryResult {
  return useQuery({
    queryKey: incidentReportsByEntryKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<IncidentReportsByEntryResponse>(
        `/election-rounds/${electionRoundId}/incident-reports:byEntry`,
        {
          params: searchParams,
        }
      );

      return {
        ...response.data,
        items: response.data.items.map((submission) => ({ ...submission, id: submission.incidentReportId })),
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

type IncidentReportsByObserverResponse = PageResponse<IncidentReportByObserver & RowData>;

type UseIncidentReportsByObserverResult = UseQueryResult<IncidentReportsByObserverResponse, Error>;

export function useIncidentReportsByObserver(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseIncidentReportsByObserverResult {
  return useQuery({
    queryKey: incidentReportsByObserverKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<IncidentReportsByObserverResponse>(
        `/election-rounds/${electionRoundId}/incident-reports:byObserver`,
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

type IncidentReportsByFormResponse = PageResponse<IncidentReportByForm & RowData>;

type UseIncidentReportsByFormResult = UseQueryResult<IncidentReportsByFormResponse, Error>;

export function useIncidentReportsByForm(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseIncidentReportsByFormResult {
  return useQuery({
    queryKey: incidentReportsAggregatedKeys.all(electionRoundId),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<{ aggregatedForms: IncidentReportByForm[] }>(
        `/election-rounds/${electionRoundId}/incident-reports:byForm`,
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

export function useIncidentReportsFilters(electionRoundId: string, dataSource: DataSources) {
  return useQuery({
    queryKey: incidentReportsByEntryKeys.filters(electionRoundId, dataSource),
    queryFn: async () => {
      const response = await authApi.get<IncidentReportsFilters>(
        `/election-rounds/${electionRoundId}/incident-reports:filters?dataSource=${dataSource}`
      );

      return response.data;
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}