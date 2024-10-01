import { authApi } from "@/common/auth-api";
import { DataTableParameters, PageResponse } from "@/common/types";
import { buildURLSearchParams } from "@/lib/utils";
import { useQuery, UseQueryResult } from "@tanstack/react-query";
import { Location } from '../models/Location';


type LocationsResult = UseQueryResult<PageResponse<Location>, Error>;

export function useLocations(electionRoundId: string, queryParams: DataTableParameters): LocationsResult {
  return useQuery({
    queryKey: ['locations', electionRoundId, queryParams],
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<PageResponse<Location>>(`/election-rounds/${electionRoundId}/locations:list`,
        {
          params: searchParams,
        }
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch locations');
      }

      return response.data;
    },
    enabled: !!electionRoundId
  });
}
