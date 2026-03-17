import { authApi } from '@/common/auth-api';
import type { SendPushNotificationRequest } from '@/features/monitoring-observers/models/push-message';
import type { PushMessageTargetedObserversSearchParams } from '@/features/monitoring-observers/models/search-params';

export function sendPushNotification(
  electionRoundId: string,
  request: SendPushNotificationRequest & { title: string; body: string }
) {
  return authApi.post<PushMessageTargetedObserversSearchParams>(
    `/election-rounds/${electionRoundId}/notifications:send`,
    request
  );
}

