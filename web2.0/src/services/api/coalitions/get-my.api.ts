import API from '@/services/api'
import type { CoalitionModel } from '@/types/coalition'

export const getCoalitionDetails = (
  electionRoundId: string
): Promise<CoalitionModel> => {
  return API.get(`/election-rounds/${electionRoundId}/coalitions:my`).then(
    (res) => res.data
  )
}
