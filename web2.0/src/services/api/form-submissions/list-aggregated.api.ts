import API from '@/services/api'
import type { PageResponse } from '@/types/common'
import type { ElectionsSearch } from '@/types/election'
import type { FormSubmissionByFormModel } from '@/types/forms-submission'
import { buildURLSearchParams } from '@/lib/utils'

export const listFormSubmissionsAggregated = async (
  electionRoundId: string,
  search: ElectionsSearch
): Promise<PageResponse<FormSubmissionByFormModel>> => {
  return API.get(
    `/election-rounds/${electionRoundId}/form-submissions:byForm`,
    {
      params: buildURLSearchParams(search),
    }
  ).then((res) => res.data)
}
