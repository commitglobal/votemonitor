import { useMutation } from '@tanstack/react-query'
import { resendMonitoringObserverInvites } from '@/services/api/monitoring-observers/resend-invites.api'

/**
 * Resending an invitation changes nothing in the list, so there is no query to
 * invalidate — the only feedback is the toast the caller raises.
 */
export const useResendMonitoringObserverInvitesMutation = (
  electionRoundId: string
) =>
  useMutation({
    mutationFn: async (observerIds: string[]) =>
      await resendMonitoringObserverInvites(electionRoundId, observerIds),
  })
