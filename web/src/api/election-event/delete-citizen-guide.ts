import { authApi } from '@/common/auth-api';

export function deleteCitizenGuide(electionRoundId: string, guideId: string) {
  return authApi.delete<void>(`/election-rounds/${electionRoundId}/citizen-guides/${guideId}`);
}

