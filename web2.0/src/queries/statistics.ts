import { queryOptions, useSuspenseQuery } from '@tanstack/react-query'
import { getElectionRoundStatistics } from '@/services/api/statistics/get.api'
import type { DataSource } from '@/types/common'
import type { MonitoringNgoStats } from '@/types/statistics'

const statisticsKeys = {
  all: (electionRoundId: string) =>
    ['election-round-statistics', electionRoundId] as const,
  /**
   * `dataSource` belongs in the key: switching between the NGO and the
   * coalition view asks the server for a different set of numbers.
   */
  byDataSource: (electionRoundId: string, dataSource: DataSource) =>
    [...statisticsKeys.all(electionRoundId), dataSource] as const,
}

const STALE_TIME = 1000 * 60 * 10 // 10 minutes

export const electionRoundStatisticsQueryOptions = <
  TResult = MonitoringNgoStats,
>(
  electionRoundId: string,
  dataSource: DataSource,
  select?: (data: MonitoringNgoStats) => TResult
) =>
  queryOptions({
    queryKey: statisticsKeys.byDataSource(electionRoundId, dataSource),
    queryFn: async () =>
      await getElectionRoundStatistics(electionRoundId, dataSource),
    staleTime: STALE_TIME,
    select,
  })

export const useSuspenseElectionRoundStatistics = <
  TResult = MonitoringNgoStats,
>(
  electionRoundId: string,
  dataSource: DataSource,
  select?: (data: MonitoringNgoStats) => TResult
) =>
  useSuspenseQuery(
    electionRoundStatisticsQueryOptions(electionRoundId, dataSource, select)
  )
