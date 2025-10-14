import { queryOptions, useQuery, useSuspenseQuery } from '@tanstack/react-query'
import { getEntriesAggregatedByForm } from '@/services/api/form-submissions/get-aggregated.api'
import { listFormSubmissionsAggregated } from '@/services/api/form-submissions/list-aggregated.api'
import { FormSubmissionsSearch } from '@/types/forms-submission'
import {
  AggregatedFormSubmissionsModel,
  AggregatedFormSubmissionsTableRow,
} from '@/types/submissions-aggregate'

export const formSubmissionsAggregatedKeys = {
  all: (electionRoundId: string) =>
    ['aggregated-form-submissions', electionRoundId] as const,
  lists: (electionRoundId: string) =>
    [...formSubmissionsAggregatedKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: FormSubmissionsSearch) =>
    [
      ...formSubmissionsAggregatedKeys.lists(electionRoundId),
      { ...params },
    ] as const,
  details: (electionRoundId: string) =>
    [...formSubmissionsAggregatedKeys.all(electionRoundId), 'detail'] as const,
  detail: (
    electionRoundId: string,
    id: string,
    params: FormSubmissionsSearch
  ) =>
    [
      ...formSubmissionsAggregatedKeys.details(electionRoundId),
      id,
      { ...params },
    ] as const,
}

const STALE_TIME = 1000 * 60 * 15 // 15 minutes

export const listFormSubmissionsAggregatedQueryOptions = <
  TResult = AggregatedFormSubmissionsTableRow[],
>(
  electionRoundId: string,
  search: FormSubmissionsSearch,
  select?: (data: AggregatedFormSubmissionsTableRow[]) => TResult
) =>
  queryOptions({
    queryKey: formSubmissionsAggregatedKeys.list(electionRoundId, search),
    queryFn: async () =>
      await listFormSubmissionsAggregated(electionRoundId, search),
    staleTime: STALE_TIME,
    select,
  })

export const useListFormSubmissionsAggregated = <
  TResult = AggregatedFormSubmissionsTableRow[],
>(
  electionRoundId: string,
  search: FormSubmissionsSearch,
  select?: (data: AggregatedFormSubmissionsTableRow[]) => TResult
) =>
  useQuery(
    listFormSubmissionsAggregatedQueryOptions(electionRoundId, search, select)
  )

export function getSubmissionsAggregatedDetailsQueryOptions<
  TResult = AggregatedFormSubmissionsModel,
>(
  electionRoundId: string,
  formId: string,
  params: FormSubmissionsSearch,
  select?: (data: AggregatedFormSubmissionsModel) => TResult
) {
  return queryOptions({
    queryKey: formSubmissionsAggregatedKeys.detail(
      electionRoundId,
      formId,
      params
    ),
    queryFn: async () =>
      await getEntriesAggregatedByForm(electionRoundId, formId, params),
    staleTime: STALE_TIME,
    select,
  })
}

export const useSuspenseGetSubmissionsAggregatedDetails = <
  TResult = AggregatedFormSubmissionsModel,
>(
  electionRoundId: string,
  formId: string,
  params: FormSubmissionsSearch,
  select?: (data: AggregatedFormSubmissionsModel) => TResult
) =>
  useSuspenseQuery(
    getSubmissionsAggregatedDetailsQueryOptions(
      electionRoundId,
      formId,
      params,
      select
    )
  )
