import { authApi } from '@/common/auth-api';

export function unstartElectionRound(electionRoundId: string) {
  return authApi.post<void>(`/election-rounds/${electionRoundId}:unstart`);
}

