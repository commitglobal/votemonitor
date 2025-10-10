import API from '@/services/api'
import type { FormSubmissionDetailedModel } from '@/types/forms-submission'

export const getSubmissionById = async (
  electionRoundId: string,
  formSubmissionId: string
): Promise<FormSubmissionDetailedModel> => {
  return API.get(
    `/election-rounds/${electionRoundId}/form-submissions/${formSubmissionId}:v2`
  ).then((res) => res.data)
}
