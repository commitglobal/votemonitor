import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams, isQueryFiltered } from '@/lib/utils';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import { Observer } from '../models/observer';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const observersKeys = {
  all: ['observers'] as const,
  lists: () => [...observersKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...observersKeys.lists(), { ...params }] as const,
  details: () => [...observersKeys.all, 'detail'] as const,
  detail: (id: string) => [...observersKeys.details(), id] as const,
};

type ObserverResponse = PageResponse<Observer>;

type UseObserversResult = UseQueryResult<ObserverResponse, Error>;

export const useObservers = (queryParams: DataTableParameters): UseObserversResult => {
  return useQuery({
    queryKey: observersKeys.list(queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<PageResponse<Observer>>(`/observers`, {
        params: searchParams,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch observers');
      }

      return {
        ...response.data,
        isEmpty: !isQueryFiltered(queryParams.otherParams ?? {}) && response.data.items.length === 0,
      };
    },
    staleTime: STALE_TIME,
  });
};
