import API from '@/services/api'
import type { GuidesObserverModel } from '@/types/guides-observer'

/**
 * Lists every observer guide of an election round.
 *
 * The endpoint takes no filter, sort or paging argument and answers with the
 * whole collection wrapped in a `guides` property, so the array is unwrapped
 * here and the table works on it locally.
 */
export const listGuidesObservers = (
  electionRoundId: string
): Promise<GuidesObserverModel[]> => {
  return API.get<{ guides: GuidesObserverModel[] }>(
    `election-rounds/${electionRoundId}/observer-guide`
  ).then((res) => res.data.guides)
}
