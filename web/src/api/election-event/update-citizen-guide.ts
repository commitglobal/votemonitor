import { authApi } from '@/common/auth-api';
import type { GuidePageType, GuideType } from '@/features/election-event/models/guide';

export type EditGuidePayload = {
  guidePageType: GuidePageType;
  guideType: GuideType;
  title: string;
  websiteUrl?: string;
  text?: string;
};

export function updateCitizenGuide(
  electionRoundId: string,
  guideId: string,
  guide: EditGuidePayload
) {
  return authApi.put<void>(`/election-rounds/${electionRoundId}/citizen-guides/${guideId}`, guide);
}

