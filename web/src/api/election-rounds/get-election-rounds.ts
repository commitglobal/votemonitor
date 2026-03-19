import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { ElectionRoundModel } from '@/features/election-rounds/models/types';
import { buildURLSearchParams } from '@/lib/utils';

export async function getElectionRounds(
  queryParams: DataTableParameters
): Promise<PageResponse<ElectionRoundModel>> {
  const params = {
    ...queryParams.otherParams,
    PageNumber: String(queryParams.pageNumber),
    PageSize: String(queryParams.pageSize),
    SortColumnName: queryParams.sortColumnName,
    SortOrder: queryParams.sortOrder,
  };

  const searchParams = buildURLSearchParams(params);

  const response = await authApi.get<PageResponse<ElectionRoundModel>>(`/election-rounds`, {
    params: searchParams,
  });

  if (response.status !== 200) {
    throw new Error('Failed to fetch election rounds');
  }

  return response.data;
}

