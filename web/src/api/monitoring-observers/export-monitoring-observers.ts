import { authApi } from '@/common/auth-api';

export async function exportMonitoringObservers(electionRoundId: string) {
  const res = await authApi.get(`/election-rounds/${electionRoundId}/monitoring-observers:export`, {
    responseType: 'blob',
  });

  return res.data as Blob;
}

