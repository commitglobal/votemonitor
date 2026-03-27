import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams, isQueryFiltered } from '@/lib/utils';
import type { MonitoringObserver } from '@/features/monitoring-observers/models/monitoring-observer';

export async function getMonitoringObservers(
  electionRoundId: string,
  queryParams: DataTableParameters
): Promise<PageResponse<MonitoringObserver> & { isEmpty: boolean }> {
  const params = {
    ...queryParams.otherParams,
    PageNumber: String(queryParams.pageNumber),
    PageSize: String(queryParams.pageSize),
    SortColumnName: queryParams.sortColumnName,
    SortOrder: queryParams.sortOrder,
  };
  const searchParams = buildURLSearchParams(params);

  const response = await authApi.get<PageResponse<MonitoringObserver>>(
    `/election-rounds/${electionRoundId}/monitoring-observers`,
    {
      params: searchParams,
    }
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch monitoring observers');
  }

  return {
    ...response.data,
    isEmpty: !isQueryFiltered(queryParams.otherParams ?? {}) && response.data.items.length === 0,
  };
}

