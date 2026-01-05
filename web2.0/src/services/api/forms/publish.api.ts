import API from '@/services/api'
import { FormModel } from '@/types/form'

export const publishForm = async (
  electionRoundId: string,
  formId: string
): Promise<FormModel> => {
  return API.put<FormModel>(
    `/election-rounds/${electionRoundId}/forms/${formId}:publish`
  ).then((res) => res.data)
}

