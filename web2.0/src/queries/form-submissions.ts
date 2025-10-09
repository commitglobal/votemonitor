import { getSubmissionById } from "@/services/api/form-submissions/get-entry.api";
import { listFormSubmissionsByEntry } from "@/services/api/form-submissions/list-entries.api";
import type { DataSource, PageResponse } from "@/types/common";
import type {
  FormSubmissionDetailedModel,
  FormSubmissionModel,
  FormSubmissionsSearch,
} from "@/types/forms-submission";
import {
  queryOptions,
  useQuery,
  useSuspenseQuery,
} from "@tanstack/react-query";

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

export function getFormSubmissionDetailsQueryOptions<
  TResult = FormSubmissionDetailedModel
>(
  electionRoundId: string,
  formSubmissionId: string,
  select?: (data: FormSubmissionDetailedModel) => TResult
) {
  return queryOptions({
    queryKey: formSubmissionKyes.detail(electionRoundId, formSubmissionId),
    queryFn: async () =>
      await getSubmissionById(electionRoundId, formSubmissionId),
    staleTime: STALE_TIME,
    select,
  });
}

export const useSuspenseGetFormSubmissionDetails = <
  TResult = FormSubmissionDetailedModel
>(
  electionRoundId: string,
  formSubmissionId: string,
  select?: (data: FormSubmissionDetailedModel) => TResult
) =>
  useSuspenseQuery(
    getFormSubmissionDetailsQueryOptions(
      electionRoundId,
      formSubmissionId,
      select
    )
  );
