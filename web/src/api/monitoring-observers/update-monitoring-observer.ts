import { authApi } from '@/common/auth-api';
import type { UpdateMonitoringObserverRequest } from '@/features/monitoring-observers/models/monitoring-observer';

export function updateMonitoringObserver(
  electionRoundId: string,
  monitoringObserverId: string,
  request: UpdateMonitoringObserverRequest
) {
  return authApi.post<void>(
    `/election-rounds/${electionRoundId}/monitoring-observers/${monitoringObserverId}`,
    request
  );
}

