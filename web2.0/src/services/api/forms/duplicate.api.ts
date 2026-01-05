import API from '@/services/api'
import { FormModel } from '@/types/form'

export const duplicateForm = async (
  electionRoundId: string,
  formId: string
): Promise<FormModel> => {
  return API.post<FormModel>(
    `/election-rounds/${electionRoundId}/forms/${formId}:duplicate`
  ).then((res) => res.data)
}

