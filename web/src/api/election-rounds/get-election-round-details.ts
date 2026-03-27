import { authApi } from '@/common/auth-api';
import type { ElectionRoundModel } from '@/features/election-rounds/models/types';

export async function getElectionRoundDetails(electionRoundId: string): Promise<ElectionRoundModel> {
  const response = await authApi.get<ElectionRoundModel>(`/election-rounds/${electionRoundId}`);

  if (response.status !== 200) {
    throw new Error('Failed to fetch election round');
  }

  return response.data;
}

