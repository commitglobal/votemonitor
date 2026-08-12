import API from '@/services/api'
import type { DataSource } from '@/types/common'
import type { MonitoringNgoStats } from '@/types/statistics'

/**
 * Fetches every number the NGO admin dashboard shows, in one request.
 *
 * `dataSource` decides whether the numbers cover the NGO alone or the whole
 * coalition it belongs to.
 */
export const getElectionRoundStatistics = (
  electionRoundId: string,
  dataSource: DataSource
): Promise<MonitoringNgoStats> => {
  return API.get<MonitoringNgoStats>(
    `/election-rounds/${electionRoundId}/statistics`,
    { params: { dataSource } }
  ).then((res) => res.data)
}
