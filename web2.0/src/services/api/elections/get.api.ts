import API from '@/services/api'
import type { ElectionModel } from '@/types/election'

export const getById = async (
  electionRoundId: string
): Promise<ElectionModel> => {
  return API.get(`/election-rounds/${electionRoundId}`).then((res) => res.data)
}
