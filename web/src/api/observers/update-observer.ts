import { authApi } from '@/common/auth-api';
import type { EditObserverFormData } from '@/features/observers/models/observer';

export function updateObserver(observerId: string, values: EditObserverFormData) {
  return authApi.put(`/observers/${observerId}`, values);
}

