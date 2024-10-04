import DoughnutChart from '@/components/charts/doughnut-chart/DoughnutChart';
import GaugeChart from '@/components/charts/gauge-chart/GaugeChart';
import LineChart from '@/components/charts/line-chart/LineChart';
import MetricChart from '@/components/charts/metric-chart/MetricChart';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ArrowDownTrayIcon } from '@heroicons/react/24/solid';
import { useRef, type ReactElement } from 'react';

import { saveChart } from '@/components/charts/utils/save-chart';
import {
  flaggedAnswersDataConfig,
  observersAccountsDataConfig,
  observersOnFieldDataConfig,
  pollingStationsDataConfig,
  questionsAnsweredDataConfig,
  quickReportsDataConfig,
  startedFormsDataConfig,
  timeSpentObservingDataConfig,
} from '../../utils/chart-defs';
export default function PlatformAdminDashboard(): ReactElement {
  const observersAccountsChartRef = useRef(null);
  const observersOnFieldChartRef = useRef(null);
  const pollingStationsChartRef = useRef(null);
  const timeSpentObservingChartRef = useRef(null);
  const startedFormsChartRef = useRef(null);
  const questionsAnsweredChartRef = useRef(null);
  const flaggedAnswersChartRef = useRef(null);
  const quickReportsChartRef = useRef(null);

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
                <DoughnutChart title='Total accounts' data={observersAccountsDataConfig} ref={observersAccountsChartRef} total={0} />
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
                  data={observersOnFieldDataConfig}
                  ref={observersOnFieldChartRef}
                  value={0}
                  total={0} />
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
                  data={pollingStationsDataConfig}
                  ref={pollingStationsChartRef}
                  value={0}
                  total={0} />
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
                  data={timeSpentObservingDataConfig}
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
                <LineChart title='forms started between 08:00 - 20:00' data={startedFormsDataConfig} ref={startedFormsChartRef} showTotal />
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
                  title='questions answered between 08:00 - 20:00'
                  data={questionsAnsweredDataConfig}
                  ref={questionsAnsweredChartRef}
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
                  title='answers were flagged through forms'
                  data={flaggedAnswersDataConfig}
                  ref={flaggedAnswersChartRef}
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
                  title='quick reports were signalled '
                  data={quickReportsDataConfig}
                  ref={quickReportsChartRef}
                  showTotal />
              </CardContent>
            </Card>
          </div>

        </div>
      </div>
    </Layout>
  )
}