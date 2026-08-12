import API from '@/services/api'

/**
 * Deletes an observer guide. The backend performs a soft delete, so the guide
 * simply stops showing up in the list.
 */
export const deleteGuide = (
  electionRoundId: string,
  guideId: string
): Promise<void> => {
  return API.delete(
    `election-rounds/${electionRoundId}/observer-guide/${guideId}`
  ).then(() => undefined)
}
