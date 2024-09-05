import { authApi } from '@/common/auth-api';
import { useQuery, UseQueryOptions, UseQueryResult } from '@tanstack/react-query';


const STALE_TIME = 1000 * 10 * 60; // 10 minutes

export const statisticsCacheKey = {
  all: (electionRoundId: string) => ['election-round-statistics', electionRoundId] as const,
}
type UseElectionRoundStatisticsResult = UseQueryResult<MonitoringStats, Error>;

export function useElectionRoundStatistics(
  electionRoundId: string
): UseElectionRoundStatisticsResult {

  return useQuery({
    queryKey: statisticsCacheKey.all(electionRoundId),
    queryFn: async () => {
      const response = await authApi.get<MonitoringStats>(`/election-rounds/${electionRoundId}/statistics`);

      return response.data;
    },
    staleTime: STALE_TIME,
  });
}