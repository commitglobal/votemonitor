import { authApi } from '@/common/auth-api';
import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { MonitoringNgoStats } from '../models/ngo-admin-statistics-models';


const STALE_TIME = 1000 * 10 * 60; // 10 minutes

export const statisticsCacheKey = {
  all: (electionRoundId: string) => ['election-round-statistics', electionRoundId] as const,
}
type UseElectionRoundStatisticsResult = UseQueryResult<MonitoringNgoStats, Error>;

export function useElectionRoundStatistics(
  electionRoundId: string
): UseElectionRoundStatisticsResult {

  return useQuery({
    queryKey: statisticsCacheKey.all(electionRoundId),
    queryFn: async () => {
      const response = await authApi.get<MonitoringNgoStats>(`/election-rounds/${electionRoundId}/statistics`);

      return response.data;
    },
    refetchOnMount: false,
    staleTime: STALE_TIME,
  });
}