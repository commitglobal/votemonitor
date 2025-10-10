import API from '@/services/api'
import { FormModel } from '@/types/form'

export const getFormById = async (
  electionRoundId: string,
  formId: string
): Promise<FormModel> => {
  return API.get(`/election-rounds/${electionRoundId}/forms/${formId}`).then(
    (res) => res.data
  )
}
