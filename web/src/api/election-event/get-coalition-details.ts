import { authApi } from '@/common/auth-api';
import type { Coalition } from '@/common/types';

export async function getCoalitionDetails(electionRoundId: string): Promise<Coalition> {
  const response = await authApi.get<Coalition>(`/election-rounds/${electionRoundId}/coalitions:my`);

  return {
    ...response.data,
  };
}

