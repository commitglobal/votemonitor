import { authApi } from "@/common/auth-api";
import type { DataTableParameters, PageResponse } from "@/common/types";
import { buildURLSearchParams, isQueryFiltered } from "@/lib/utils";
import { type UseQueryResult, useQuery } from "@tanstack/react-query";
import type { TargetedMonitoringObserver } from "../models/targeted-monitoring-observer";
import type { PushMessageModel } from "../models/push-message";

const STALE_TIME = 1000 * 5; // five minutes

export const pushMessagesKeys = {
  all: ['push-messages'] as const,
  lists: () => [...pushMessagesKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...pushMessagesKeys.lists(), { ...params }] as const,
  details: () => [...pushMessagesKeys.all, 'detail'] as const,
  detail: (id: string) => [...pushMessagesKeys.details(), id] as const,
}

export const targetedObserversKeys = {
  all: ['pm-targeted-monitoring-observers'] as const,
  lists: () => [...pushMessagesKeys.all, 'list'] as const,
  list: (params: DataTableParameters) => [...pushMessagesKeys.lists(), { ...params }] as const,
}

type PushMessageResponse = PageResponse<PushMessageModel>;

type UsePushMessagesResult = UseQueryResult<PushMessageResponse, Error>;

export function usePushMessages(queryParams: DataTableParameters): UsePushMessagesResult {
  return useQuery({
    queryKey: pushMessagesKeys.list(queryParams),
    queryFn: async () => {
      const electionRoundId = localStorage.getItem('electionRoundId');

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
  });
}

type ListTargetedMonitoringObserverResponse = PageResponse<TargetedMonitoringObserver>;

type UseTargetedMonitoringObserversResult = UseQueryResult<ListTargetedMonitoringObserverResponse, Error>;


export const useTargetedMonitoringObservers = (queryParams: DataTableParameters): UseTargetedMonitoringObserversResult => {
  return useQuery({
    queryKey: [targetedObserversKeys.list(queryParams)],
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi
        .get<PageResponse<TargetedMonitoringObserver>>(`election-rounds/${electionRoundId}/notifications:listRecipients`,
          {
            params: searchParams,
          }
        );

      if (response.status !== 200) {
        throw new Error('Failed to fetch monitoring observers');
      }

      return response.data;
    },
  });
};
