import { authApi } from '@/common/auth-api';

export function deleteMonitoringNgo(electionRoundId: string, ngoId: string) {
  return authApi.delete(`election-rounds/${electionRoundId}/monitoring-ngos/${ngoId}`);
}

