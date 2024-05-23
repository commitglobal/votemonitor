import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { FormSubmissionByEntry, FormSubmissionByForm, FormSubmissionByObserver } from '../models/form-submission';
import { authApi } from '@/common/auth-api';
import { buildURLSearchParams } from '@/lib/utils';
import type { RowData } from '@/components/ui/DataTable/DataTable';

const STALE_TIME = 1000 * 60; // one minute

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

export const formSubmissionsAggregtedKeys = {
  all: ['aggregated-form-submissions'] as const,
  lists: () => [...formSubmissionsAggregtedKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...formSubmissionsAggregtedKeys.lists(), { ...params }] as const,
  details: () => [...formSubmissionsAggregtedKeys.all, 'detail'] as const,
  detail: (id: string) => [...formSubmissionsAggregtedKeys.details(), id] as const,
}

type FormSubmissionsByEntryResponse = PageResponse<FormSubmissionByEntry & RowData>;

type UseFormSubmissionsByEntryResult = UseQueryResult<FormSubmissionsByEntryResponse, Error>;

export function useFormSubmissionsByEntry(queryParams: DataTableParameters): UseFormSubmissionsByEntryResult {
  return useQuery({
    queryKey: formSubmissionsByEntryKeys.list(queryParams),
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

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
  });
}

type FormSubmissionsByObserverResponse = PageResponse<FormSubmissionByObserver & RowData>;

type UseFormSubmissionsByObserverResult = UseQueryResult<FormSubmissionsByObserverResponse, Error>;

export function useFormSubmissionsByObserver(queryParams: DataTableParameters): UseFormSubmissionsByObserverResult {
  return useQuery({
    queryKey: formSubmissionsByObserverKeys.list(queryParams),
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

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
  });
}

type FormSubmissionsByFormResponse = PageResponse<FormSubmissionByForm & RowData>;

type UseFormSubmissionsByFormResult = UseQueryResult<FormSubmissionsByFormResponse, Error>;

export function useFormSubmissionsByForm(queryParams: DataTableParameters): UseFormSubmissionsByFormResult {
  return useQuery({
    queryKey: formSubmissionsAggregtedKeys.all,
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

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
  });
}
