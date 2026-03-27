import { authApi } from '@/common/auth-api';
import type { MonitoringNgoModel } from '@/features/election-rounds/models/types';

type MonitoringNgosPageResponse = {
  monitoringNgos: MonitoringNgoModel[];
};

export async function getMonitoringNgos(
  electionRoundId: string
): Promise<MonitoringNgosPageResponse> {
  const response = await authApi.get<MonitoringNgosPageResponse>(
    `election-rounds/${electionRoundId}/monitoring-ngos`
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch monitoring NGOs for election round');
  }

  return response.data;
}

