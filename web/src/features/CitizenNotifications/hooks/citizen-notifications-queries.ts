import { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams, isQueryFiltered } from '@/lib/utils';
import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { CitizenNotificationModel } from '../models/citizen-notification';
import API from '@/services/api';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const citizenNotificationsKeys = {
  all: (electionRoundId: string) => ['citizen-notifications', electionRoundId] as const,
  lists: (electionRoundId: string) => [...citizenNotificationsKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...citizenNotificationsKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...citizenNotificationsKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...citizenNotificationsKeys.details(electionRoundId), id] as const,
};

type CitizenNotificationResponse = PageResponse<CitizenNotificationModel>;

type UseCitizenNotificationsResult = UseQueryResult<CitizenNotificationResponse, Error>;

export function useCitizenNotifications(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseCitizenNotificationsResult {
  return useQuery({
    queryKey: citizenNotificationsKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await API.get<CitizenNotificationResponse>(
        `/election-rounds/${electionRoundId}/citizen-notifications:listSent`,
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
