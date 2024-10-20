import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams, isQueryFiltered } from '@/lib/utils';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import { MonitoringObserver } from '../models/monitoring-observer';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const monitoringObserversKeys = {
  all: (electionRoundId: string) => ['monitoring-observers', electionRoundId] as const,
  lists: (electionRoundId: string) => [...monitoringObserversKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...monitoringObserversKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...monitoringObserversKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...monitoringObserversKeys.details(electionRoundId), id] as const,
  tags: (electionRoundId: string) => [...monitoringObserversKeys.details(electionRoundId), 'tags'] as const,
};

type MonitoringObserverResponse = PageResponse<MonitoringObserver>;

type UseMonitoringObserversResult = UseQueryResult<MonitoringObserverResponse, Error>;

export const useMonitoringObservers = (
  electionRoundId: string,
  queryParams: DataTableParameters
): UseMonitoringObserversResult => {
  return useQuery({
    queryKey: monitoringObserversKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
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
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME
  });
};
