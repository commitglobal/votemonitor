import { authApi } from "@/common/auth-api";
import { DataTableParameters, PageResponse } from "@/common/types";
import { buildURLSearchParams } from "@/lib/utils";
import { UseQueryResult, useQuery } from "@tanstack/react-query";

const STALE_TIME = 1000 * 60; // one minute

type PushMessageResponse = PageResponse<PushMessageModel>;

type UsePushMessagesResult = UseQueryResult<PushMessageResponse, Error>;


export const formTemplatesKeys = {
    all: ['push-messages'] as const,
    lists: () => [...formTemplatesKeys.all, 'list'] as const,
    list: (params: DataTableParameters) => [...formTemplatesKeys.lists(), { ...params }] as const,
    details: () => [...formTemplatesKeys.all, 'detail'] as const,
    detail: (id: string) => [...formTemplatesKeys.details(), id] as const,
}

export function usePushMessages(queryParams: DataTableParameters): UsePushMessagesResult {
  return useQuery({
    queryKey: formTemplatesKeys.list(queryParams),
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
      };
    },
    staleTime: STALE_TIME,
  });
}
