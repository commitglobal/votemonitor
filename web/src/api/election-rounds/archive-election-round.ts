import { authApi } from '@/common/auth-api';

export function archiveElectionRound(electionRoundId: string) {
  return authApi.post<void>(`/election-rounds/${electionRoundId}:archive`);
}

