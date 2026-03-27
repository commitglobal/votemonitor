import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { Observer } from '@/features/observers/models/observer';

export async function getObservers(
  queryParams: DataTableParameters
): Promise<PageResponse<Observer>> {
  const response = await authApi.get<PageResponse<Observer>>('/observers', {
    params: {
      ...queryParams.otherParams,
      status: (queryParams.otherParams as any)?.observerStatus,
    },
  });

  if (response.status !== 200) {
    throw new Error('Failed to fetch observers');
  }

  return response.data;
}

