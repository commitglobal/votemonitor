import DoughnutChart from '@/components/charts/doughnut-chart/DoughnutChart';
import GaugeChart from '@/components/charts/gauge-chart/GaugeChart';
import LineChart from '@/components/charts/line-chart/LineChart';
import MetricChart from '@/components/charts/metric-chart/MetricChart';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ArrowDownTrayIcon } from '@heroicons/react/24/solid';
import { useMemo, useRef } from 'react';

import type { FunctionComponent } from '@/common/types';
import { saveChart } from '@/components/charts/utils/save-chart';
import {
  histogramChartConfig,
  observersAccountsDataConfig,
  observersOnTheFieldDataConfig,
  pollingStationsDataConfig,
  timeSpentObservingDataConfig,
} from '../../utils/chart-defs';

import { useQuery } from '@tanstack/react-query';
import { authApi } from '@/common/auth-api';
import { format } from 'date-fns';
import { useTranslation } from 'react-i18next';
const STALE_TIME = 1000 * 60 * 10; // ten minutes

export default function NgoAdminDashboard(): FunctionComponent {
  const { t } = useTranslation();
  const observersAccountsChartRef = useRef(null);
  const observersOnFieldChartRef = useRef(null);
  const pollingStationsChartRef = useRef(null);
  const timeSpentObservingChartRef = useRef(null);
  const startedFormsChartRef = useRef(null);
  const questionsAnsweredChartRef = useRef(null);
  const flaggedAnswersChartRef = useRef(null);
  const quickReportsChartRef = useRef(null);

  const { data: statistics } = useQuery({
    queryKey: ['statistics'],
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<MonitoringStats>(`/election-rounds/${electionRoundId}/statistics`);

      return response.data;
    },
    staleTime: STALE_TIME,
  });

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
    <Layout title={t('ngoAdminDashboard.title')} subtitle={t('ngoAdminDashboard.subtitle')}>
      <div className="flex-col md:flex">
        <div className="flex-1 space-y-4">
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between py-0!">
                <CardTitle className="text-sm font-medium">
                  {t('ngoAdminDashboard.observersAccountsCardTitle')}
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(observersAccountsChartRef, 'observers-accounts.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <DoughnutChart
                  title={t('ngoAdminDashboard.observersAccountsIndicatorTitle')}
                  total={statistics?.observersStats?.totalNumberOfObservers ?? 0}
                  data={observersAccountsDataConfig(statistics?.observersStats)}
                  ref={observersAccountsChartRef} />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between py-0!">
                <CardTitle className="text-sm font-medium">
                  {t('ngoAdminDashboard.observersOnFieldCardTitle')}
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(observersOnFieldChartRef, 'observers-on-field.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <GaugeChart
                  title={t('ngoAdminDashboard.observersInPollingStationsIndicatorTitle')}
                  metricLabel={t('ngoAdminDashboard.observersInPollingStationsIndicatorMetricLabel')}
                  data={observersOnTheFieldDataConfig(statistics?.observersStats?.totalNumberOfObservers, statistics?.numberOfObserversOnTheField)}
                  value={statistics?.numberOfObserversOnTheField ?? 0}
                  total={statistics?.observersStats?.totalNumberOfObservers ?? 0}
                  ref={observersOnFieldChartRef} />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  {t('ngoAdminDashboard.pollingStationCardTitle')}
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(pollingStationsChartRef, 'polling-stations-covered.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <GaugeChart
                  title={t('ngoAdminDashboard.stationsVisitedByAtLeastOneObserverIndicatorTitle')}
                  metricLabel={t('ngoAdminDashboard.stationsVisitedByAtLeastOneObserverMetricLabel')}
                  data={pollingStationsDataConfig(statistics?.pollingStationsStats)}
                  total={statistics?.pollingStationsStats.totalNumberOfPollingStations ?? 0}
                  value={statistics?.pollingStationsStats.numberOfVisitedPollingStations ?? 0}
                  ref={pollingStationsChartRef} />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  {t('ngoAdminDashboard.timeSpentObservingCardTitle')}
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(timeSpentObservingChartRef, 'time-spent-observing.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <MetricChart
                  title={t('ngoAdminDashboard.basedOnStartEndTimesReportedIndicatorTitle')}
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
                  {t('ngoAdminDashboard.startedFormsCardTitle')}
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(startedFormsChartRef, 'started-forms.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <LineChart
                  title={`${t('ngoAdminDashboard.formsStartedBetweenIndicatorTitle')} ${getInterval(statistics?.formsHistogram)}`}
                  data={histogramChartConfig(statistics?.formsHistogram)}
                  ref={startedFormsChartRef}
                  total={getTotal(statistics?.formsHistogram)}
                  showTotal />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  {t('ngoAdminDashboard.questionsAnsweredCardTitle')}
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(questionsAnsweredChartRef, 'questions-answered.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <LineChart
                  title={`${t('ngoAdminDashboard.questionsAnsweredBetweenIndicatorTitle')} ${getInterval(statistics?.formsHistogram)}`}
                  data={histogramChartConfig(statistics?.questionsHistogram)}
                  ref={questionsAnsweredChartRef}
                  total={getTotal(statistics?.questionsHistogram)}
                  showTotal />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  {t('ngoAdminDashboard.flaggedAnswersCardTitle')}
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(flaggedAnswersChartRef, 'flagged-answers.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <LineChart
                  title={`${t('ngoAdminDashboard.answersWereFlaggedThroughFormsBetweenIndicatorTitle')} ${getInterval(statistics?.formsHistogram)}`}
                  data={histogramChartConfig(statistics?.flaggedAnswersHistogram, 'red')}
                  ref={flaggedAnswersChartRef}
                  total={getTotal(statistics?.flaggedAnswersHistogram)}
                  showTotal />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle className="text-sm font-medium">
                  {t('ngoAdminDashboard.quickReportsCardTitle')}
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => { saveChart(quickReportsChartRef, 'quick-reports.png') }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <LineChart
                  title={`${t('ngoAdminDashboard.quickReportsWereSignalledBetweenIndicatorTitle')} ${getInterval(statistics?.formsHistogram)}`}
                  data={histogramChartConfig(statistics?.quickReportsHistogram, 'red')}
                  ref={quickReportsChartRef}
                  total={getTotal(statistics?.quickReportsHistogram)}
                  showTotal />
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </Layout>
  )
}