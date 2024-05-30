import type { DataTableParameters, PageResponse } from '@/common/types';
const STALE_TIME = 1000 * 60 * 5; // one minute

// type FormSubmissionsByEntryResponse = PageResponse<FormSubmissionByEntry>;

// type UseFormSubmissionsByEntryResult = UseQueryResult<FormSubmissionsByEntryResponse, Error>;

// type FormSubmissionsByObserverResponse = PageResponse<FormSubmissionByObserver>;

// type UseFormSubmissionsByObserverResult = UseQueryResult<FormSubmissionsByObserverResponse, Error>;

// export function useFormSubmissionsByObserver(queryParams: DataTableParameters): UseFormSubmissionsByObserverResult {
//   return useQuery({
//     queryKey: ['form-submissions', 'by-observer', queryParams],
//     queryFn: async () => {
//       const electionRoundId = localStorage.getItem('electionRoundId');

//       const params = {
//         ...queryParams.otherParams,
//         PageNumber: String(queryParams.pageNumber),
//         PageSize: String(queryParams.pageSize),
//         SortColumnName: queryParams.sortColumnName,
//         SortOrder: queryParams.sortOrder,
//       };
//       const searchParams = new URLSearchParams(params);

//       const response = await authApi.get<FormSubmissionsByObserverResponse>(
//         `/election-rounds/${electionRoundId}/form-submissions:byObserver`,
//         {
//           params: searchParams,
//         }
//       );

//       return {
//         ...response.data,
//         items: response.data.items.map((submission) => ({ ...submission, id: submission.monitoringObserverId })),
//       };
//     },
//     staleTime: STALE_TIME,
//   });
// }

// type FormSubmissionsByFormResponse = PageResponse<FormSubmissionByForm>;

// type UseFormSubmissionsByFormResult = UseQueryResult<FormSubmissionsByFormResponse, Error>;

// export function useFormSubmissionsByForm(queryParams: DataTableParameters): UseFormSubmissionsByFormResult {
//   return useQuery({
//     queryKey: ['form-submissions', 'by-form', queryParams],
//     queryFn: async () => {
//       const electionRoundId = localStorage.getItem('electionRoundId');

//       const params = {
//         ...queryParams.otherParams,
//         PageNumber: String(queryParams.pageNumber),
//         PageSize: String(queryParams.pageSize),
//         SortColumnName: queryParams.sortColumnName,
//         SortOrder: queryParams.sortOrder,
//       };
//       const searchParams = new URLSearchParams(params);

//       const response = await authApi.get<{ aggregatedForms: FormSubmissionByForm[] }>(
//         `/election-rounds/${electionRoundId}/form-submissions:byForm`,
//         {
//           params: searchParams,
//         }
//       );

//       return {
//         currentPage: 1,
//         pageSize: queryParams.pageSize,
//         totalCount: response.data.aggregatedForms.length,
//         items: response.data.aggregatedForms.map((submission) => ({
//           ...submission,
//           id: submission.formId,
//         })),
//       };
//     },
//     staleTime: STALE_TIME,
//   });
// }
