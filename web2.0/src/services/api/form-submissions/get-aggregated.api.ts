import API from '@/services/api'
import type {
  FormSubmissionDetailedModel,
  FormSubmissionsSearch,
} from '@/types/forms-submission'
import { buildURLSearchParams } from '@/lib/utils'

export const getEntriesAggregatedByForm = async (
  electionRoundId: string,
  formId: string,
  search: FormSubmissionsSearch
): Promise<FormSubmissionDetailedModel> => {
  return API.get(
    `/election-rounds/${electionRoundId}/form-submissions/${formId}:aggregated`,
    {
      params: buildURLSearchParams(search),
    }
  ).then((res) => res.data)
}
