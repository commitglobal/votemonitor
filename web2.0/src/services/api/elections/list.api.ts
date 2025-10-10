import API from '@/services/api'
import type { PageResponse } from '@/types/common'
import type { ElectionModel, ElectionsSearch } from '@/types/election'
import { buildURLSearchParams } from '@/lib/utils'

export const listElections = (
  search: ElectionsSearch
): Promise<PageResponse<ElectionModel>> => {
  return API.get(`/election-rounds`, {
    params: buildURLSearchParams(search),
  }).then((res) => res.data)
}
