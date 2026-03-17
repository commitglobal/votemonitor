import { authApi } from '@/common/auth-api';
import type { ElectionEvent } from '@/features/election-event/models/election-event';

export async function getElectionEvent(electionRoundId: string): Promise<ElectionEvent> {
  const response = await authApi.get<ElectionEvent>(`/election-rounds/${electionRoundId}`);

  return {
    ...response.data,
  };
}

