import z from 'zod'
import { DataSource } from './common'

/**
 * Search params of the dashboard.
 *
 * `dataSource` is the only one the server reads: it switches the whole page
 * between the NGO's own numbers and the coalition's.
 */
export const dashboardSearchSchema = z.object({
  dataSource: z.enum(DataSource).default(DataSource.Ngo),
})

/**
 * A single bucket of a time histogram.
 *
 * `bucket` is an ISO timestamp in UTC: the server groups by hour, so the local
 * hour has to be derived when the value is rendered on an axis.
 */
export interface HistogramEntry {
  bucket: string
  value: number
}

/** Breakdown of the observer accounts of the monitoring NGO. */
export interface ObserversStats {
  activeObservers: number
  pendingObservers: number
  suspendedObservers: number
  totalNumberOfObservers: number
}

/**
 * Aggregated numbers for one administrative level.
 *
 * The same shape carries the totals of the whole election round (`totalStats`)
 * and the per level breakdowns, where `path` names the location.
 */
export interface VisitedPollingStationLevelStats {
  path: string
  level: number
  numberOfPollingStations: number
  numberOfVisitedPollingStations: number
  coveragePercentage: number
  activeObservers: number
  numberOfFormSubmissions: number
  numberOfQuestionsAnswered: number
  numberOfFlaggedAnswers: number
  numberOfQuickReports: number
  numberOfIncidentReports: number
  /** Total observing time, in minutes. */
  minutesMonitoring: number
}

/** Everything the dashboard renders, returned by a single request. */
export interface MonitoringNgoStats {
  observersStats: ObserversStats
  totalStats?: VisitedPollingStationLevelStats
  level1Stats: VisitedPollingStationLevelStats[]
  level2Stats: VisitedPollingStationLevelStats[]
  level3Stats: VisitedPollingStationLevelStats[]
  level4Stats: VisitedPollingStationLevelStats[]
  level5Stats: VisitedPollingStationLevelStats[]
  formsHistogram: HistogramEntry[]
  questionsHistogram: HistogramEntry[]
  flaggedAnswersHistogram: HistogramEntry[]
  quickReportsHistogram: HistogramEntry[]
  citizenReportsHistogram: HistogramEntry[]
  incidentReportsHistogram: HistogramEntry[]
}
