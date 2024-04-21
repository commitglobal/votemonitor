import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { FormSubmissionByEntry, FormSubmissionByForm, FormSubmissionByObserver } from '../models/form-submission';
import { authApi } from '@/common/auth-api';

type FormSubmissionsByEntryResponse = PageResponse<FormSubmissionByEntry>;

type UseFormSubmissionsByEntryResult = UseQueryResult<FormSubmissionsByEntryResponse, Error>;

export function useFormSubmissionsByEntry(queryParams: DataTableParameters): UseFormSubmissionsByEntryResult {
  return useQuery({
    queryKey: ['form-submissions-by-entry', queryParams],
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

      const params = {
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = new URLSearchParams(params);

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
  });
}

type FormSubmissionsByObserverResponse = PageResponse<FormSubmissionByObserver>;

type UseFormSubmissionsByObserverResult = UseQueryResult<FormSubmissionsByObserverResponse, Error>;

export function useFormSubmissionsByObserver(queryParams: DataTableParameters): UseFormSubmissionsByObserverResult {
  return useQuery({
    queryKey: ['form-submissions-by-observer', queryParams],
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

      const params = {
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = new URLSearchParams(params);

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
  });
}

type FormSubmissionsByFormResponse = { formSubmissionsAggregates: FormSubmissionByForm[] };

type UseFormSubmissionsByFormResult = UseQueryResult<FormSubmissionsByFormResponse, Error>;

export function useFormSubmissionsByForm(queryParams: DataTableParameters): UseFormSubmissionsByFormResult {
  return useQuery({
    queryKey: ['form-submissions-by-form', queryParams],
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

      const params = {
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = new URLSearchParams(params);

      const response = await authApi.get<FormSubmissionsByFormResponse>(
        `/election-rounds/${electionRoundId}/form-submissions:byForm`,
        {
          params: searchParams,
        }
      );

      return {
        ...response.data,
        items: response.data.formSubmissionsAggregates.map((submission) => ({
          ...submission,
          id: submission.formId,
        })),
      };
    },
  });
}
