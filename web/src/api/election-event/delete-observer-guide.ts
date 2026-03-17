import { authApi } from '@/common/auth-api';

export function deleteObserverGuide(electionRoundId: string, guideId: string) {
  return authApi.delete<void>(`/election-rounds/${electionRoundId}/observer-guide/${guideId}`);
}

