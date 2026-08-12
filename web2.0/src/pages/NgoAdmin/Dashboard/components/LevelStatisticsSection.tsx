import { useMemo } from 'react'
import type {
  MonitoringNgoStats,
  VisitedPollingStationLevelStats,
} from '@/types/statistics'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import {
  LevelStatisticsCard,
  type LevelStatsEntry,
} from './LevelStatisticsCard'

/** Rounds to one decimal, which is as precise as these figures need to read. */
const round = (value: number) => Math.round(value * 10) / 10

/**
 * The metrics broken down per location, in the order they appear on the level.
 *
 * Each one pulls a single number out of the level's rows and drops the
 * locations that have nothing to report, so a card never fills with zeroes.
 */
const metrics: {
  title: string
  fileName: string
  select: (stats: VisitedPollingStationLevelStats) => number
  unit?: string
}[] = [
  {
    title: 'Observers on the field',
    fileName: 'observers-on-field',
    select: (stats) => stats.activeObservers,
  },
  {
    title: 'Questions answered',
    fileName: 'questions-answered',
    select: (stats) => stats.numberOfQuestionsAnswered,
  },
  {
    title: 'Flagged answers',
    fileName: 'flagged-answers',
    select: (stats) => stats.numberOfFlaggedAnswers,
  },
  {
    title: 'Visited polling stations',
    fileName: 'visited-polling-stations',
    select: (stats) => stats.numberOfVisitedPollingStations,
  },
  {
    title: 'Polling stations coverage',
    fileName: 'polling-stations-coverage',
    select: (stats) => round(stats.coveragePercentage),
    unit: '%',
  },
  {
    title: 'Time spent observing',
    fileName: 'time-spent-observing',
    // Reported in minutes by the API, read in hours by people.
    select: (stats) => round(stats.minutesMonitoring / 60),
    unit: ' h',
  },
  {
    title: 'Quick reports',
    fileName: 'quick-reports',
    select: (stats) => stats.numberOfQuickReports,
  },
]

type LevelStatisticsSectionProps = {
  statistics: MonitoringNgoStats
}

/**
 * Per-location breakdowns, grouped by administrative level.
 *
 * How many levels a country has varies, so only the ones the API actually
 * returned get a tab — an empty "Level 4" would suggest missing data rather
 * than a country with three levels.
 */
export function LevelStatisticsSection({
  statistics,
}: LevelStatisticsSectionProps) {
  const levels = useMemo(() => {
    const byLevel = [
      statistics.level1Stats,
      statistics.level2Stats,
      statistics.level3Stats,
      statistics.level4Stats,
      statistics.level5Stats,
    ]

    return byLevel
      .map((stats, index) => ({ level: index + 1, stats: stats ?? [] }))
      .filter((entry) => entry.stats.length > 0)
  }, [statistics])

  if (levels.length === 0) {
    return null
  }

  return (
    <Tabs defaultValue={`level-${levels[0].level}`}>
      <TabsList>
        {levels.map(({ level }) => (
          <TabsTrigger key={level} value={`level-${level}`}>
            Level {level}
          </TabsTrigger>
        ))}
      </TabsList>

      {levels.map(({ level, stats }) => (
        <TabsContent key={level} value={`level-${level}`} className='mt-4'>
          <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
            {metrics.map((metric) => {
              const entries: LevelStatsEntry[] = stats
                .map((row) => ({
                  path: row.path,
                  value: metric.select(row),
                  unit: metric.unit,
                }))
                .filter((entry) => entry.value > 0)

              return (
                <LevelStatisticsCard
                  key={metric.title}
                  title={metric.title}
                  entries={entries}
                  fileName={`level-${level}-${metric.fileName}`}
                />
              )
            })}
          </div>
        </TabsContent>
      ))}
    </Tabs>
  )
}
