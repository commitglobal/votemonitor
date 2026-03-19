import { authApi } from '@/common/auth-api';
import type { GuideModel } from '@/features/election-event/models/guide';

type ObserverGuidesResponse = {
  guides: GuideModel[];
};

export async function getObserverGuides(electionRoundId: string): Promise<ObserverGuidesResponse> {
  const response = await authApi.get<ObserverGuidesResponse>(
    `/election-rounds/${electionRoundId}/observer-guide`
  );

  return response.data;
}

