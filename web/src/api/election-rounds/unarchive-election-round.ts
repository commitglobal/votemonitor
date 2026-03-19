import { authApi } from '@/common/auth-api';

export function unarchiveElectionRound(electionRoundId: string) {
  return authApi.post<void>(`/election-rounds/${electionRoundId}:unarchive`);
}

