import { authApi } from '@/common/auth-api';
import type { GuideModel } from '@/features/election-event/models/guide';

type CitizenGuidesResponse = {
  guides: GuideModel[];
};

export async function getCitizenGuides(electionRoundId: string): Promise<CitizenGuidesResponse> {
  const response = await authApi.get<CitizenGuidesResponse>(
    `/election-rounds/${electionRoundId}/citizen-guides`
  );

  return response.data;
}

