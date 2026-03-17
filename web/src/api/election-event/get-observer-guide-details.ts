import { authApi } from '@/common/auth-api';
import type { GuideModel } from '@/features/election-event/models/guide';

export async function getObserverGuideDetails(
  electionRoundId: string,
  guideId: string
): Promise<GuideModel> {
  const response = await authApi.get<GuideModel>(
    `/election-rounds/${electionRoundId}/observer-guide/${guideId}`
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch observer guide details');
  }

  return response.data;
}

