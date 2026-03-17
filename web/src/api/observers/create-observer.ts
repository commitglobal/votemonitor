import { authApi } from '@/common/auth-api';
import type { AddObserverFormData } from '@/features/observers/models/observer';

export function createObserver(values: AddObserverFormData) {
  return authApi.post(`/observers`, values);
}

