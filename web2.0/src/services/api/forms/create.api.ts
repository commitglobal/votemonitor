import API from '@/services/api'
import { FormModel } from '@/types/form'

export type CreateFormRequest = Omit<
  FormModel,
  | 'id'
  | 'questions'
  | 'lastModifiedOn'
  | 'lastModifiedBy'
  | 'numberOfQuestions'
  | 'languagesTranslationStatus'
  | 'isFormOwner'
  | 'formAccess'
  | 'status'
>

export const createForm = async (
  electionRoundId: string,
  form: CreateFormRequest
): Promise<FormModel> => {
  return API.post<FormModel>(
    `/election-rounds/${electionRoundId}/forms`,
    form
  ).then((res) => res.data)
}
