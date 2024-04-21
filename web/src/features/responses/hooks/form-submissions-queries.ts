import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { FormSubmissionByEntry } from '../models/form-submission';
import { authApi } from '@/common/auth-api';

type FormSubmissionsByEntryResponse = PageResponse<FormSubmissionByEntry>;

type UseFormSubmissionsByEntryResult = UseQueryResult<FormSubmissionsByEntryResponse, Error>;

export function useFormSubmissionsByEntry(queryParams: DataTableParameters): UseFormSubmissionsByEntryResult {
  return useQuery({
    queryKey: ['form-submissions', queryParams],
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
