import API from '@/services/api'
import { FormSubmissionsSearch } from '@/types/forms-submission'
import { AggregatedFormSubmissionsModel } from '@/types/submissions-aggregate'
import { buildURLSearchParams } from '@/lib/utils'

export const getEntriesAggregatedByForm = async (
  electionRoundId: string,
  formId: string,
  search: FormSubmissionsSearch
): Promise<AggregatedFormSubmissionsModel> => {
  return API.get(
    `/election-rounds/${electionRoundId}/form-submissions/${formId}:aggregated`,
    {
      params: buildURLSearchParams(search),
    }
  ).then((res) => res.data)
}
