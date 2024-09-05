import DoughnutChart from '@/components/charts/doughnut-chart/DoughnutChart';
import GaugeChart from '@/components/charts/gauge-chart/GaugeChart';
import LineChart from '@/components/charts/line-chart/LineChart';
import MetricChart from '@/components/charts/metric-chart/MetricChart';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ArrowDownTrayIcon } from '@heroicons/react/24/solid';
import { useRef } from 'react';

import type { FunctionComponent } from '@/common/types';
import { saveChart } from '@/components/charts/utils/save-chart';
import {
  histogramChartConfig,
  observersAccountsDataConfig,
  observersOnTheFieldDataConfig,
  pollingStationsDataConfig,
  timeSpentObservingDataConfig,
} from '../../utils/chart-defs';

import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { format } from 'date-fns';
import { useElectionRoundStatistics } from '../../hooks/statistics-queries';

export default function NgoAdminDashboard(): FunctionComponent {
  const observersAccountsChartRef = useRef(null);
  const observersOnFieldChartRef = useRef(null);
  const pollingStationsChartRef = useRef(null);
  const timeSpentObservingChartRef = useRef(null);
  const startedFormsChartRef = useRef(null);
  const questionsAnsweredChartRef = useRef(null);
  const flaggedAnswersChartRef = useRef(null);
  const quickReportsChartRef = useRef(null);
  const citizenReportsChartRef = useRef(null);

  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const isMonitoringNgoForCitizenReporting = useCurrentElectionRoundStore(s => s.isMonitoringNgoForCitizenReporting);

  const { data: statistics } = useElectionRoundStatistics(currentElectionRoundId);

  function getInterval(histogram: HistogramEntry[] | undefined): string {
    if (histogram) {
      const data = histogram.map(x => new Date(x.bucket)).map(date => date.getTime());
      const minDate = new Date(Math.min(...data));

      // Get the maximum date
      const maxDate = new Date(Math.max(...data));

      return `${format(minDate, 'kk:00')} - ${format(maxDate, 'kk:00')}`;
    }
    return '-'
  }

  function getTotal(formsHistogram: HistogramEntry[] | undefined): number {
    return (formsHistogram ?? []).map(x => x.value).reduce((accumulator, currentValue) => accumulator + currentValue, 0);
  }

  return (
    <Layout title='Dashboard' subtitle='Key indicators.'>
      <div className="flex-col md:flex">
        <div className="flex-1 space-y-4">
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between py-0!">
                <CardTitle className="text-sm font-medium">
                  Observers accounts
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(observersAccountsChartRef, 'observers-accounts.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <DoughnutChart
                  title='Total accounts'
                  total={statistics?.observersStats?.totalNumberOfObservers ?? 0}
                  data={observersAccountsDataConfig(statistics?.observersStats)}
                  ref={observersAccountsChartRef} />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between py-0!">
                <CardTitle className="text-sm font-medium">
                  Observers on field
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(observersOnFieldChartRef, 'observers-on-field.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <GaugeChart
                  title='Observers in polling stations'
                  metricLabel='With at least one question answered'
                  data={observersOnTheFieldDataConfig(statistics?.observersStats?.totalNumberOfObservers, statistics?.numberOfObserversOnTheField)}
                  value={statistics?.numberOfObserversOnTheField ?? 0}
                  total={statistics?.observersStats?.totalNumberOfObservers ?? 0}
                  ref={observersOnFieldChartRef} />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  Polling stations
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(pollingStationsChartRef, 'polling-stations-covered.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <GaugeChart
                  title='Stations visited by at least one observer'
                  metricLabel='coverage'
                  data={pollingStationsDataConfig(statistics?.pollingStationsStats)}
                  total={statistics?.pollingStationsStats.totalNumberOfPollingStations ?? 0}
                  value={statistics?.pollingStationsStats.numberOfVisitedPollingStations ?? 0}
                  ref={pollingStationsChartRef} />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  Time spent observing
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(timeSpentObservingChartRef, 'time-spent-observing.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <MetricChart
                  title='Based on start-end times reported'
                  unit='h'
                  data={timeSpentObservingDataConfig(statistics?.minutesMonitoring)}
                  ref={timeSpentObservingChartRef} />
              </CardContent>
            </Card>
          </div>

          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  Started forms
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(startedFormsChartRef, 'started-forms.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <LineChart
                  title={`forms started between ${getInterval(statistics?.formsHistogram)}`}
                  data={histogramChartConfig(statistics?.formsHistogram)}
                  ref={startedFormsChartRef}
                  total={getTotal(statistics?.formsHistogram)}
                  showTotal />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  Questions answered
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(questionsAnsweredChartRef, 'questions-answered.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <LineChart
                  title={`questions answered between ${getInterval(statistics?.formsHistogram)}`}
                  data={histogramChartConfig(statistics?.questionsHistogram)}
                  ref={questionsAnsweredChartRef}
                  total={getTotal(statistics?.questionsHistogram)}
                  showTotal />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  Flagged answers
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(flaggedAnswersChartRef, 'flagged-answers.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <LineChart
                  title={`answers were flagged through forms between ${getInterval(statistics?.formsHistogram)}`}
                  data={histogramChartConfig(statistics?.flaggedAnswersHistogram, 'red')}
                  ref={flaggedAnswersChartRef}
                  total={getTotal(statistics?.flaggedAnswersHistogram)}
                  showTotal />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  Quick reports
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(quickReportsChartRef, 'quick-reports.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <LineChart
                  title={`quick reports were signalled between ${getInterval(statistics?.quickReportsHistogram)}`}
                  data={histogramChartConfig(statistics?.quickReportsHistogram, 'red')}
                  ref={quickReportsChartRef}
                  total={getTotal(statistics?.quickReportsHistogram)}
                  showTotal />
              </CardContent>
            </Card>
            {isMonitoringNgoForCitizenReporting &&
              <Card>
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle className="text-sm font-medium">
                    Citizen reports
                  </CardTitle>
                  <Button type='button' variant='ghost' onClick={() => { saveChart(quickReportsChartRef, 'quick-reports.png') }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                </CardHeader>
                <CardContent>
                  <LineChart
                    title={`citizen reports were signalled between ${getInterval(statistics?.citizenReportsHistogram)}`}
                    data={histogramChartConfig(statistics?.citizenReportsHistogram, 'red')}
                    ref={citizenReportsChartRef}
                    total={getTotal(statistics?.citizenReportsHistogram)}
                    showTotal />
                </CardContent>
              </Card>}
          </div>
        </div>
      </div>
    </Layout>
  )
}