import { useListMonitoringElections } from '@/queries/monitoring-elections'
import { useSuspenseElectionRoundStatistics } from '@/queries/statistics'
import { Route } from '@/routes/(app)/elections/$electionRoundId'
import { H1, P } from '@/components/ui/typography'
import { HistogramCard } from './components/HistogramCard'
import { LevelStatisticsSection } from './components/LevelStatisticsSection'
import { MeterCard } from './components/MeterCard'
import { ObserverAccountsCard } from './components/ObserverAccountsCard'
import { StatCard } from './components/StatCard'

/** The API reports observing time in minutes; the dashboard speaks in hours. */
const toHours = (minutes: number) => Math.round((minutes / 60) * 10) / 10

function Page() {
  const { electionRoundId } = Route.useParams()
  const { dataSource } = Route.useSearch()

  // Whether this NGO runs citizen reporting for the round is only carried by
  // the monitored-elections list, which the round switcher already keeps warm
  // in the cache, so reading it here costs no extra request.
  const { data: monitoredElections } = useListMonitoringElections()
  const isCitizenReportingNgo =
    monitoredElections?.some(
      (election) =>
        election.id === electionRoundId &&
        election.isMonitoringNgoForCitizenReporting
    ) ?? false

  // One request feeds the whole page, so every card below reads from the same
  // snapshot instead of drifting apart.
  const { data: statistics } = useSuspenseElectionRoundStatistics(
    electionRoundId,
    dataSource
  )

  const totalStats = statistics.totalStats

  return (
    <div className='flex flex-col gap-6'>
      <div>
        <H1>Statistics</H1>
        <P>Here&apos;s how your election round is going</P>
      </div>

      <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
        <ObserverAccountsCard stats={statistics.observersStats} />

        <MeterCard
          title='Observers on the field'
          value={totalStats?.activeObservers ?? 0}
          total={statistics.observersStats?.totalNumberOfObservers ?? 0}
          caption='of the observers are active'
          reachedLabel='On the field'
          remainingLabel='Not on the field'
          fileName='observers-on-field'
        />

        <MeterCard
          title='Polling stations covered'
          value={totalStats?.numberOfVisitedPollingStations ?? 0}
          total={totalStats?.numberOfPollingStations ?? 0}
          caption='of the stations were visited'
          reachedLabel='Visited'
          remainingLabel='Not visited'
          fileName='polling-stations-covered'
        />

        <StatCard
          title='Time spent observing'
          value={toHours(totalStats?.minutesMonitoring ?? 0).toLocaleString()}
          caption='hours in total'
        />
      </div>

      <div className='grid gap-4 md:grid-cols-2'>
        <HistogramCard
          title='Started forms'
          histogram={statistics.formsHistogram}
          fileName='started-forms'
        />
        <HistogramCard
          title='Questions answered'
          histogram={statistics.questionsHistogram}
          fileName='questions-answered'
        />
        <HistogramCard
          title='Flagged answers'
          histogram={statistics.flaggedAnswersHistogram}
          fileName='flagged-answers'
          tone='negative'
        />
        <HistogramCard
          title='Quick reports'
          histogram={statistics.quickReportsHistogram}
          fileName='quick-reports'
          tone='negative'
        />
        {/* Citizen reports only exist for the NGO that runs them for this
            election round, so the card would otherwise sit empty forever. */}
        {isCitizenReportingNgo ? (
          <HistogramCard
            title='Citizen reports'
            histogram={statistics.citizenReportsHistogram}
            fileName='citizen-reports'
            tone='negative'
          />
        ) : null}
      </div>

      <LevelStatisticsSection statistics={statistics} />
    </div>
  )
}

export default Page
