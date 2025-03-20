import { authApi } from '@/common/auth-api';
import { ElectionRoundStatus, PageResponse } from '@/common/types';
import { UseQueryResult, useQuery } from '@tanstack/react-query';
import { MonitoringNgoModel } from '../../models/types';
import { electionRoundKeys } from '../../queries';
const STALE_TIME = 1000 * 60 * 15; // fifteen minutes

export interface ElectionsRoundsQueryParams {
  searchText: string | undefined;
  countryId: string | undefined;
  electionRoundStatus: ElectionRoundStatus | undefined;
}

export function useMonitoringNgos(electionRoundId: string): UseQueryResult<PageResponse<MonitoringNgoModel>, Error> {
  return useQuery({
    queryKey: electionRoundKeys.detail(electionRoundId),
    queryFn: async () => {
      const response = await authApi.get<MonitoringNgoModel>(`/election-rounds/${electionRoundId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch monitoring NGOs for election round');
      }

      return response.data;
    },
    enabled: !!electionRoundId,

    staleTime: STALE_TIME,
  });
}
