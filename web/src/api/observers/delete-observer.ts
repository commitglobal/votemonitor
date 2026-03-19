import { authApi } from '@/common/auth-api';

export function deleteObserver(observerId: string) {
  return authApi.delete(`/observers/${observerId}`);
}

