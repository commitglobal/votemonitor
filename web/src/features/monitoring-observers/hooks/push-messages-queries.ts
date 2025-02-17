import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams, isQueryFiltered } from '@/lib/utils';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { TargetedMonitoringObserver } from '../models/targeted-monitoring-observer';
import type { PushMessageModel } from '../models/push-message';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const pushMessagesKeys = {
  all: (electionRoundId: string) => ['push-messages', electionRoundId] as const,
  lists: (electionRoundId: string) => [...pushMessagesKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...pushMessagesKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...pushMessagesKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...pushMessagesKeys.details(electionRoundId), id] as const,
};

export const targetedObserversKeys = {
  all: (electionRoundId: string) => ['pm-targeted-monitoring-observers', electionRoundId] as const,
  lists: (electionRoundId: string) => [...targetedObserversKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...targetedObserversKeys.lists(electionRoundId), { ...params }] as const,
};

type PushMessageResponse = PageResponse<PushMessageModel>;

type UsePushMessagesResult = UseQueryResult<PushMessageResponse, Error>;

export function usePushMessages(electionRoundId: string, queryParams: DataTableParameters): UsePushMessagesResult {
  return useQuery({
    queryKey: pushMessagesKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<PushMessageResponse>(
        `/election-rounds/${electionRoundId}/notifications:listSent`,
        {
          params: searchParams,
        }
      );

      return {
        ...response.data,
        isEmpty: !isQueryFiltered(params) && response.data.items.length === 0,
      };
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
}

type ListTargetedMonitoringObserverResponse = PageResponse<TargetedMonitoringObserver>;

type UseTargetedMonitoringObserversResult = UseQueryResult<ListTargetedMonitoringObserverResponse, Error>;

export const useTargetedMonitoringObservers = (
  electionRoundId: string,
  queryParams: DataTableParameters
): UseTargetedMonitoringObserversResult => {
  return useQuery({
    queryKey: targetedObserversKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
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
    },
    staleTime: STALE_TIME,
    enabled: !!electionRoundId,
  });
};
