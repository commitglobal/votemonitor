import { authApi } from '@/common/auth-api';
import type { EditGuidePayload } from './update-citizen-guide';

export function updateObserverGuide(
  electionRoundId: string,
  guideId: string,
  guide: EditGuidePayload
) {
  return authApi.put<void>(`/election-rounds/${electionRoundId}/observer-guide/${guideId}`, guide);
}

