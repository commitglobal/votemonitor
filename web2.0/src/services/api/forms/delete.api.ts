import API from '@/services/api'

export const deleteForm = async (
  electionRoundId: string,
  formId: string
): Promise<void> => {
  return API.delete(`/election-rounds/${electionRoundId}/forms/${formId}`).then((res) => res.data)
}

