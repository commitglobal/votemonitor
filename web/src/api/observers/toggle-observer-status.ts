import { authApi } from '@/common/auth-api';

export function toggleObserverStatus(observerId: string, isObserverActive: boolean) {
  const ACTION = isObserverActive ? 'deactivate' : 'activate';

  return authApi.put(`/observers/${observerId}:${ACTION}`, {});
}

