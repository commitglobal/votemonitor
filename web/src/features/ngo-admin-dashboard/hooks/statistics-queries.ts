import { authApi } from '@/common/auth-api';
import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { MonitoringNgoStats } from '../models/ngo-admin-statistics-models';
import { DataSources } from '@/common/types';


const STALE_TIME = 1000 * 10 * 60; // 10 minutes

export const statisticsCacheKey = {
  all: (electionRoundId: string, dataSource: DataSources) => ['election-round-statistics', electionRoundId, dataSource] as const,
}
type UseElectionRoundStatisticsResult = UseQueryResult<MonitoringNgoStats, Error>;

export function useElectionRoundStatistics(
  electionRoundId: string,
  dataSource: DataSources
): UseElectionRoundStatisticsResult {

  return useQuery({
    queryKey: statisticsCacheKey.all(electionRoundId, dataSource),
    queryFn: async () => {
      const response = await authApi.get<MonitoringNgoStats>(`/election-rounds/${electionRoundId}/statistics?dataSource=${dataSource}`);

      return response.data;
    },
    refetchOnMount: false,
    staleTime: STALE_TIME,
  });
}