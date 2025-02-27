import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse, PollingStation } from '@/common/types';
import { pollingStationsKeys } from '@/hooks/polling-stations-levels';
import { buildURLSearchParams } from '@/lib/utils';
import { useMutation, useQuery, UseQueryResult } from '@tanstack/react-query';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export function usePollingStations(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseQueryResult<PageResponse<PollingStation>, Error> {
  return useQuery({
    queryKey: pollingStationsKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<PageResponse<PollingStation>>(
        `/election-rounds/${electionRoundId}/polling-stations:list`,
        {
          params: searchParams,
        }
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch polling stations');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
}

export function useUpdatePollingStationMutation() {
  return useMutation({
    mutationFn: ({
      electionRoundId,
      pollingStationId,
      pollingStation,
    }: {
      electionRoundId: string;
      pollingStationId: string;
      pollingStation: PollingStation;
      onSuccess?: () => void;
      onError?: () => void;
    }) => {
      return authApi.put<PollingStation>(
        `/election-rounds/${electionRoundId}/polling-stations/${pollingStationId}`,
        pollingStation
      );
    },

    onSuccess: (_, { onSuccess }) => onSuccess?.(),

    onError: (_, { onError }) => onError?.(),
  });
}

export function useDeletePollingStationMutation() {
  return useMutation({
    mutationFn: ({
      electionRoundId,
      pollingStationId,
    }: {
      electionRoundId: string;
      pollingStationId: string;
      onSuccess?: () => void;
      onError?: () => void;
    }) => {
      return authApi.delete<PollingStation>(`/election-rounds/${electionRoundId}/polling-stations/${pollingStationId}`);
    },

    onSuccess: (_, { onSuccess }) => onSuccess?.(),

    onError: (_, { onError }) => onError?.(),
  });
}
