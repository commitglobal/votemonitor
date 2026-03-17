import { authApi } from '@/common/auth-api';
import type { DataSources } from '@/common/types';
import type { MonitoringNgoStats } from '@/features/ngo-admin-dashboard/models/ngo-admin-statistics-models';

export async function getElectionRoundStatistics(
  electionRoundId: string,
  dataSource: DataSources,
): Promise<MonitoringNgoStats> {
  const response = await authApi.get<MonitoringNgoStats>(
    `/election-rounds/${electionRoundId}/statistics?dataSource=${dataSource}`,
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch election round statistics');
  }

  return response.data;
}

