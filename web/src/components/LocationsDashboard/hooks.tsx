import API from '@/services/api';
import { DataTableParameters, PageResponse, Location } from '@/common/types';
import { locationsKeys } from '@/hooks/locations-levels';
import { buildURLSearchParams } from '@/lib/utils';
import { useMutation, useQuery, UseQueryResult } from '@tanstack/react-query';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export function useLocations(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseQueryResult<PageResponse<Location>, Error> {
  return useQuery({
    queryKey: locationsKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await API.get<PageResponse<Location>>(`/election-rounds/${electionRoundId}/locations:list`, {
        params: searchParams,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch locations');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
}

export function useUpdateLocationMutation() {
  return useMutation({
    mutationFn: ({
      electionRoundId,
      locationId,
      location,
    }: {
      electionRoundId: string;
      locationId: string;
      location: Location;
      onSuccess?: () => void;
      onError?: () => void;
    }) => {
      return API.put<Location>(`/election-rounds/${electionRoundId}/locations/${locationId}`, location);
    },

    onSuccess: (_, { onSuccess }) => onSuccess?.(),

    onError: (_, { onError }) => onError?.(),
  });
}

export function useDeleteLocationMutation() {
  return useMutation({
    mutationFn: ({
      electionRoundId,
      locationId,
    }: {
      electionRoundId: string;
      locationId: string;
      onSuccess?: () => void;
      onError?: () => void;
    }) => {
      return API.delete<Location>(`/election-rounds/${electionRoundId}/locations/${locationId}`);
    },

    onSuccess: (_, { onSuccess }) => onSuccess?.(),

    onError: (_, { onError }) => onError?.(),
  });
}
