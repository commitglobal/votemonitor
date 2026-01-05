import API from '@/services/api'
import { FormModel } from '@/types/form'

export const obsoleteForm = async (
  electionRoundId: string,
  formId: string
): Promise<FormModel> => {
  return API.post<FormModel>(
    `/election-rounds/${electionRoundId}/forms/${formId}:obsolete`
  ).then((res) => res.data)
}

