import { getEntryById } from "@/services/api/form-submissions/get-entry.api";
import { listFormSubmissionsByEntry } from "@/services/api/form-submissions/list-entries.api";
import type { DataSource, PageResponse } from "@/types/common";
import type {
  FormSubmissionModel,
  FormSubmissionsSearch,
} from "@/types/forms-submission";
import { queryOptions, useQuery } from "@tanstack/react-query";

export const formSubmissionKyes = {
  all: (electionRoundId: string) =>
    ["form-submissions", electionRoundId] as const,
  lists: (electionRoundId: string) =>
    [...formSubmissionKyes.all(electionRoundId), "list"] as const,
  list: (electionRoundId: string, params: FormSubmissionsSearch) =>
    [...formSubmissionKyes.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) =>
    [...formSubmissionKyes.all(electionRoundId), "detail"] as const,
  detail: (electionRoundId: string, formSubmissionId: string) =>
    [...formSubmissionKyes.details(electionRoundId), formSubmissionId] as const,
  filters: (electionRoundId: string, dataSource: DataSource) =>
    [
      ...formSubmissionKyes.details(electionRoundId),
      dataSource,
      "filters",
    ] as const,
};

const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const listFormSubmissionsQueryOptions = <
  TResult = PageResponse<FormSubmissionModel>
>(
  electionRoundId: string,
  search: FormSubmissionsSearch,
  select?: (data: PageResponse<FormSubmissionModel>) => TResult
) =>
  queryOptions({
    queryKey: formSubmissionKyes.list(electionRoundId, search),
    queryFn: async () =>
      await listFormSubmissionsByEntry(electionRoundId, search),
    staleTime: STALE_TIME,
    select,
  });

export const useListFormSubmissions = <
  TResult = PageResponse<FormSubmissionModel>
>(
  electionRoundId: string,
  search: FormSubmissionsSearch,
  select?: (data: PageResponse<FormSubmissionModel>) => TResult
) => useQuery(listFormSubmissionsQueryOptions(electionRoundId, search, select));

export function getFormSubmissionDetailsQueryOptions(
  electionRoundId: string,
  formSubmissionId: string
) {
  return queryOptions({
    queryKey: formSubmissionKyes.detail(electionRoundId, formSubmissionId),
    queryFn: async () => await getEntryById(electionRoundId, formSubmissionId),
    staleTime: STALE_TIME,
  });
}
