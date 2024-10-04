import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { RowData } from '@/components/ui/DataTable/DataTable';
import { buildURLSearchParams } from '@/lib/utils';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import type { FormSubmissionByEntry, FormSubmissionByForm, FormSubmissionByObserver } from '../models/form-submission';

const STALE_TIME = 1000 * 60; // one minute

// export const formSubmissionsByEntryKeys = {
//   all: (electionRoundId: string) => ['form-submissions-by-entry', electionRoundId] as const,
//   lists: (electionRoundId: string) => [...formSubmissionsByEntryKeys.all(electionRoundId), 'list'] as const,
//   list: (electionRoundId: string, params: DataTableParameters) => [...formSubmissionsByEntryKeys.lists(electionRoundId), { ...params }] as const,
//   details: (electionRoundId: string) => [...formSubmissionsByEntryKeys.all(electionRoundId), 'detail'] as const,
//   detail: (electionRoundId: string, id: string) => [...formSubmissionsByEntryKeys.details(electionRoundId), id] as const,
// }

// export const formSubmissionsByObserverKeys = {
//   all: (electionRoundId: string) => ['form-submissions-by-observer', electionRoundId] as const,
//   lists: (electionRoundId: string) => [...formSubmissionsByObserverKeys.all(electionRoundId), 'list'] as const,
//   list: (electionRoundId: string, params: DataTableParameters) => [...formSubmissionsByObserverKeys.lists(electionRoundId), { ...params }] as const,
//   details: (electionRoundId: string) => [...formSubmissionsByObserverKeys.all(electionRoundId), 'detail'] as const,
//   detail: (electionRoundId: string, id: string) => [...formSubmissionsByObserverKeys.details(electionRoundId), id] as const,
// }

// export const formSubmissionsAggregatedKeys = {
//   all: (electionRoundId: string) => ['aggregated-form-submissions', electionRoundId] as const,
//   lists: (electionRoundId: string) => [...formSubmissionsAggregatedKeys.all(electionRoundId), 'list'] as const,
//   list: (electionRoundId: string, params: DataTableParameters) => [...formSubmissionsAggregatedKeys.lists(electionRoundId), { ...params }] as const,
//   details: (electionRoundId: string) => [...formSubmissionsAggregatedKeys.all(electionRoundId), 'detail'] as const,
//   detail: (electionRoundId: string, id: string) => [...formSubmissionsAggregatedKeys.details(electionRoundId), id] as const,
// }

export const formSubmissionsByEntryKeys = {
  all: ['form-submissions-by-entry'] as const,
  lists: () => [...formSubmissionsByEntryKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...formSubmissionsByEntryKeys.lists(), { ...params }] as const,
  details: () => [...formSubmissionsByEntryKeys.all, 'detail'] as const,
  detail: (id: string) => [...formSubmissionsByEntryKeys.details(), id] as const,
}

export const formSubmissionsByObserverKeys = {
  all: ['form-submissions-by-observer'] as const,
  lists: () => [...formSubmissionsByObserverKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...formSubmissionsByObserverKeys.lists(), { ...params }] as const,
  details: () => [...formSubmissionsByObserverKeys.all, 'detail'] as const,
  detail: (id: string) => [...formSubmissionsByObserverKeys.details(), id] as const,
}

export const formSubmissionsAggregatedKeys = {
  all: ['aggregated-form-submissions'] as const,
  lists: () => [...formSubmissionsAggregatedKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...formSubmissionsAggregatedKeys.lists(), { ...params }] as const,
  details: () => [...formSubmissionsAggregatedKeys.all, 'detail'] as const,
  detail: (id: string) => [...formSubmissionsAggregatedKeys.details(), id] as const,
}


type FormSubmissionsByEntryResponse = PageResponse<FormSubmissionByEntry & RowData>;

type UseFormSubmissionsByEntryResult = UseQueryResult<FormSubmissionsByEntryResponse, Error>;

export function useFormSubmissionsByEntry(electionRoundId: string, queryParams: DataTableParameters): UseFormSubmissionsByEntryResult {
  return useQuery({
    queryKey: formSubmissionsByEntryKeys.list(queryParams),
    queryFn: async () => {

      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<FormSubmissionsByEntryResponse>(
        `/election-rounds/${electionRoundId}/form-submissions:byEntry`,
        {
          params: searchParams,
        }
      );

      return {
        ...response.data,
        items: response.data.items.map((submission) => ({ ...submission, id: submission.submissionId })),
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId
  });
}

type FormSubmissionsByObserverResponse = PageResponse<FormSubmissionByObserver & RowData>;

type UseFormSubmissionsByObserverResult = UseQueryResult<FormSubmissionsByObserverResponse, Error>;

export function useFormSubmissionsByObserver(electionRoundId: string, queryParams: DataTableParameters): UseFormSubmissionsByObserverResult {
  return useQuery({
    queryKey: formSubmissionsByObserverKeys.list(queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<FormSubmissionsByObserverResponse>(
        `/election-rounds/${electionRoundId}/form-submissions:byObserver`,
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
    enabled: !!electionRoundId
  });
}

type FormSubmissionsByFormResponse = PageResponse<FormSubmissionByForm & RowData>;

type UseFormSubmissionsByFormResult = UseQueryResult<FormSubmissionsByFormResponse, Error>;

export function useFormSubmissionsByForm(electionRoundId: string, queryParams: DataTableParameters): UseFormSubmissionsByFormResult {
  return useQuery({
    queryKey: formSubmissionsAggregatedKeys.all,
    queryFn: async () => {

      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<{ aggregatedForms: FormSubmissionByForm[] }>(
        `/election-rounds/${electionRoundId}/form-submissions:byForm`,
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
    enabled: !!electionRoundId
  });
}
