import { authApi } from '@/common/auth-api';

export function resendMonitoringObserverInvites(
  electionRoundId: string,
  monitoringObserverIds: (string | undefined)[]
) {
  return authApi.put<void>(`/election-rounds/${electionRoundId}/monitoring-observers:resend-invites`, {
    ids: monitoringObserverIds.filter((id) => !!id),
  });
}

