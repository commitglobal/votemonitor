import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { MonitoringNgoModel } from '@/features/election-rounds/models/types';

export async function getAvailableMonitoringNgos(
  electionRoundId: string,
  params: DataTableParameters
): Promise<PageResponse<MonitoringNgoModel>> {
  const response = await authApi.get<PageResponse<MonitoringNgoModel>>(
    `election-rounds/${electionRoundId}/monitoring-ngos:available`,
    {
      params: {
        ...params.otherParams,
      },
    }
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch ngo admins');
  }

  return response.data;
}

