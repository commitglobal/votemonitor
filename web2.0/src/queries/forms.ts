import { queryOptions, useQuery, useSuspenseQuery } from '@tanstack/react-query'
import { getFormById } from '@/services/api/forms/get-by-id'
import { listForms } from '@/services/api/forms/list.api'
import type { PageResponse } from '@/types/common'
import { FormModel, FormSearch } from '@/types/form'

export const formsKeys = {
  all: (electionRoundId: string) => ['forms', electionRoundId] as const,
  lists: (electionRoundId: string) =>
    [...formsKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: FormSearch) =>
    [...formsKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) =>
    [...formsKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...formsKeys.details(electionRoundId), id] as const,
}

const STALE_TIME = 1000 * 60 * 15 // 15 minutes

export const listFormsQueryOptions = <TResult = PageResponse<FormModel>>(
  electionRoundId: string,
  search: FormSearch,
  select?: (data: PageResponse<FormModel>) => TResult
) =>
  queryOptions({
    queryKey: formsKeys.list(electionRoundId, search),
    queryFn: async () => await listForms(electionRoundId, search),
    staleTime: STALE_TIME,
    select,
  })

export const useListForms = <TResult = PageResponse<FormModel>>(
  electionRoundId: string,
  search: FormSearch,
  select?: (data: PageResponse<FormModel>) => TResult
) => useQuery(listFormsQueryOptions(electionRoundId, search, select))

export function getFormDetailsQueryOptions<TResult = FormModel>(
  electionRoundId: string,
  formId: string,
  select?: (data: FormModel) => TResult
) {
  return queryOptions({
    queryKey: formsKeys.detail(electionRoundId, formId),
    queryFn: async () => await getFormById(electionRoundId, formId),
    staleTime: STALE_TIME,
    select,
  })
}

export const useSuspenseGetFormDetails = <TResult = FormModel>(
  electionRoundId: string,
  formId: string,
  select?: (data: FormModel) => TResult
) =>
  useSuspenseQuery(getFormDetailsQueryOptions(electionRoundId, formId, select))
