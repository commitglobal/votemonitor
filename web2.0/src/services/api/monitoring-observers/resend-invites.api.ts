import API from '@/services/api'

/**
 * Sends the invitation email again.
 *
 * The endpoint takes a list because it also serves bulk resending from the
 * observers table; a single row passes an array of one.
 */
export const resendMonitoringObserverInvites = (
  electionRoundId: string,
  observerIds: string[]
): Promise<void> => {
  return API.put(
    `/election-rounds/${electionRoundId}/monitoring-observers:resend-invites`,
    { ids: observerIds }
  ).then(() => undefined)
}
