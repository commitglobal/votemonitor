import { authApi } from '@/common/auth-api';

export type CreateMonitoringObserverPayload = {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  tags: string[];
};

export function createMonitoringObservers(
  electionRoundId: string,
  observers: CreateMonitoringObserverPayload[]
) {
  return authApi.post(`/election-rounds/${electionRoundId}/monitoring-observers`, { observers });
}

