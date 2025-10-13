import API from '@/services/api'
import { FormSubmissionsSearch } from '@/types/forms-submission'
import { AggregatedFormSubmissionsTableRow } from '@/types/submissions-aggregate'
import { buildURLSearchParams } from '@/lib/utils'

export const listFormSubmissionsAggregated = async (
  electionRoundId: string,
  search: FormSubmissionsSearch
): Promise<AggregatedFormSubmissionsTableRow[]> => {
  return API.get<{ aggregatedForms: AggregatedFormSubmissionsTableRow[] }>(
    `/election-rounds/${electionRoundId}/form-submissions:byForm`,
    {
      params: buildURLSearchParams(search),
    }
  ).then((res) => res.data.aggregatedForms)
}
