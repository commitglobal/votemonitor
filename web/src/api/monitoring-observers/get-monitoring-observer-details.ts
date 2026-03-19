import { authApi } from '@/common/auth-api';
import type { MonitoringObserver } from '@/features/monitoring-observers/models/monitoring-observer';

export async function getMonitoringObserverDetails(
  electionRoundId: string,
  monitoringObserverId: string
): Promise<MonitoringObserver> {
  const response = await authApi.get<MonitoringObserver>(
    `/election-rounds/${electionRoundId}/monitoring-observers/${monitoringObserverId}`
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch monitoring observer details');
  }

  return response.data;
}

