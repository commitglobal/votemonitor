import { authApi } from '@/common/auth-api';
import type { PushMessageDetailedModel } from '@/features/monitoring-observers/models/push-message';

export async function getPushMessageDetails(
  electionRoundId: string,
  pushMessageId: string
): Promise<PushMessageDetailedModel> {
  const response = await authApi.get<PushMessageDetailedModel>(
    `/election-rounds/${electionRoundId}/notifications/${pushMessageId}`
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch notification details');
  }

  return response.data;
}

