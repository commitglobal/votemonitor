import API from '@/services/api'
import { FormModel } from '@/types/form'
import { Language } from '@/types/language'

export const updateFormLanguages = async (
  electionRoundId: string,
  formId: string,
  languageCodes: Language[]
): Promise<FormModel> => {
  return API.put<FormModel>(
    `/election-rounds/${electionRoundId}/forms/${formId}:addTranslations`,
    { languageCodes }
  ).then((res) => res.data)
}

