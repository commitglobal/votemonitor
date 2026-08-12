import API from '@/services/api'

export type UpdateGuideRequest = {
  title: string
  /** Only read by the backend for `Website` guides. */
  websiteUrl?: string
  /** Only read by the backend for `Text` guides. */
  text?: string
}

/**
 * Updates an observer guide.
 *
 * The guide type and the uploaded file are fixed at creation time, so a
 * `Document` guide can only be renamed through this call. The endpoint answers
 * with `204 No Content`, which is why nothing is returned here.
 */
export const updateGuide = (
  electionRoundId: string,
  guideId: string,
  guide: UpdateGuideRequest
): Promise<void> => {
  return API.put(
    `election-rounds/${electionRoundId}/observer-guide/${guideId}`,
    guide
  ).then(() => undefined)
}
