import API from '@/services/api'
import type { PageResponse } from '@/types/common'
import { FormModel, FormSearch } from '@/types/form'
import { buildURLSearchParams } from '@/lib/utils'

export const listForms = async (
  electionRoundId: string,
  search: FormSearch
): Promise<PageResponse<FormModel>> => {
  return API.get(`/election-rounds/${electionRoundId}/forms`, {
    params: buildURLSearchParams(search),
  }).then((res) => res.data)
}
