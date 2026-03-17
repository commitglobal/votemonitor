import { authApi } from '@/common/auth-api';

export function addMonitoringNgo(electionRoundId: string, ngoId: string) {
  return authApi.post(`election-rounds/${electionRoundId}/monitoring-ngos`, { ngoId });
}

