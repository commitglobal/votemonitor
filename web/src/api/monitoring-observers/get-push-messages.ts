import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams, isQueryFiltered } from '@/lib/utils';
import type { PushMessageModel } from '@/features/monitoring-observers/models/push-message';

export async function getPushMessages(
  electionRoundId: string,
  queryParams: DataTableParameters
): Promise<PageResponse<PushMessageModel> & { isEmpty: boolean }> {
  const params = {
    ...queryParams.otherParams,
    PageNumber: String(queryParams.pageNumber),
    PageSize: String(queryParams.pageSize),
    SortColumnName: queryParams.sortColumnName,
    SortOrder: queryParams.sortOrder,
  };
  const searchParams = buildURLSearchParams(params);

  const response = await authApi.get<PageResponse<PushMessageModel>>(
    `/election-rounds/${electionRoundId}/notifications:listSent`,
    {
      params: searchParams,
    }
  );

  return {
    ...response.data,
    isEmpty: !isQueryFiltered(params) && response.data.items.length === 0,
  };
}

