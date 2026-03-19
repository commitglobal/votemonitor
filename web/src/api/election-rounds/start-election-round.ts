import { authApi } from '@/common/auth-api';

export function startElectionRound(electionRoundId: string) {
  return authApi.post<void>(`/election-rounds/${electionRoundId}:start`);
}

