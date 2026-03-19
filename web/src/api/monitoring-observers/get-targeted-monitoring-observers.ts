import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams } from '@/lib/utils';
import type { TargetedMonitoringObserver } from '@/features/monitoring-observers/models/targeted-monitoring-observer';

export async function getTargetedMonitoringObservers(
  electionRoundId: string,
  queryParams: DataTableParameters
): Promise<PageResponse<TargetedMonitoringObserver>> {
  const params = {
    ...queryParams.otherParams,
    PageNumber: String(queryParams.pageNumber),
    PageSize: String(queryParams.pageSize),
    SortColumnName: queryParams.sortColumnName,
    SortOrder: queryParams.sortOrder,
  };
  const searchParams = buildURLSearchParams(params);

  const response = await authApi.get<PageResponse<TargetedMonitoringObserver>>(
    `election-rounds/${electionRoundId}/notifications:listRecipients`,
    {
      params: searchParams,
    }
  );

  if (response.status !== 200) {
    throw new Error('Failed to fetch notification');
  }

  return response.data;
}

