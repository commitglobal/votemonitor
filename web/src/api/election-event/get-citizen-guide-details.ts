import { authApi } from '@/common/auth-api';
import type { GuideModel } from '@/features/election-event/models/guide';

export async function getCitizenGuideDetails(
  electionRoundId: string,
  guideId: string
): Promise<GuideModel> {
  const response = await authApi.get<GuideModel>(
    `/election-rounds/${electionRoundId}/citizen-guides/${guideId}`
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch citizen guide details');
  }

  return response.data;
}

