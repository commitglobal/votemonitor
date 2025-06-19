import API from '@/services/api';
import { DataTableParameters, PageResponse, PollingStation } from '@/common/types';
import { ImportPollingStationRow } from '@/features/polling-stations/PollingStationsImport/PollingStationsImport';
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

      const response = await API.get<PageResponse<PollingStation>>(
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

export const useDeletePollingStationMutation = (onSuccess?: () => void, onError?: () => void) => {
  return useMutation({
    mutationFn: ({ electionRoundId, pollingStationId }: { electionRoundId: string; pollingStationId: string }) => {
      return API.delete<PollingStation>(`/election-rounds/${electionRoundId}/polling-stations/${pollingStationId}`);
    },

    onSuccess: () => onSuccess?.(),
    onError: () => onError?.(),
  });
};

export const useUpsertPollingStation = (onSucces?: () => void, onError?: () => void) =>
  useMutation({
    mutationFn: ({ electionRoundId, values }: { electionRoundId: string; values: ImportPollingStationRow }) => {
      return API.post(`/election-rounds/${electionRoundId}/polling-stations`, {
        pollingStations: [values],
      });
    },
    onSuccess: onSucces,
    onError: onError,
  });
