import { authApi } from '@/common/auth-api';
import type { Observer } from '@/features/observers/models/observer';

export async function getObserverDetails(observerId: string): Promise<Observer> {
  const response = await authApi.get<Observer>(`/observers/${observerId}`);

  if (response.status !== 200) {
    throw new Error('Failed to fetch observer details');
  }

  return response.data;
}

