import DoughnutChart from '@/components/charts/doughnut-chart/DoughnutChart';
import GaugeChart from '@/components/charts/gauge-chart/GaugeChart';
import MetricChart from '@/components/charts/metric-chart/MetricChart';
import TimeLineChart from '@/components/charts/time-line-chart/TimeLineChart';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ArrowDownTrayIcon, ArrowsPointingInIcon, ArrowsPointingOutIcon } from '@heroicons/react/24/solid';
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

import { DateTimeHourBucketFormat } from '@/common/formats';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { useTranslation } from 'react-i18next';
import { useElectionRoundStatistics } from '../../hooks/statistics-queries';
import useDashboardExpandedChartsStore from './dashboard-config.store';

export default function NgoAdminDashboard(): FunctionComponent {
  const { t } = useTranslation();
  const { expandedCharts, toggleChart } = useDashboardExpandedChartsStore();

  const observersAccountsChartRef = useRef(null);
  const observersOnFieldChartRef = useRef(null);
  const pollingStationsChartRef = useRef(null);
  const timeSpentObservingChartRef = useRef(null);
  const startedFormsChartRef = useRef(null);
  const questionsAnsweredChartRef = useRef(null);
  const flaggedAnswersChartRef = useRef(null);
  const quickReportsChartRef = useRef(null);
  const citizenReportsChartRef = useRef(null);
  const incidentReportsChartRef = useRef(null);

  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const isMonitoringNgoForCitizenReporting = useCurrentElectionRoundStore((s) => s.isMonitoringNgoForCitizenReporting);

  const { data: statistics } = useElectionRoundStatistics(currentElectionRoundId);

  function getInterval(histogram: HistogramEntry[] | undefined): string {
    if (histogram && histogram.some((x) => x)) {
      const data = histogram.map((x) => new Date(x.bucket)).map((date) => date.getTime());
      const minDate = new Date(Math.min(...data));

      // Get the maximum date
      const maxDate = new Date(Math.max(...data));

      return `${format(minDate, DateTimeHourBucketFormat)} - ${format(maxDate, DateTimeHourBucketFormat)}`;
    }
    return '-';
  }

  function getTotal(formsHistogram: HistogramEntry[] | undefined): number {
    return (formsHistogram ?? [])
      .map((x) => x.value)
      .reduce((accumulator, currentValue) => accumulator + currentValue, 0);
  }

  return (
    <Layout title={t('ngoAdminDashboard.title')} subtitle={t('ngoAdminDashboard.subtitle')}>
      <div className='flex-col md:flex'>
        <div className='flex-1 space-y-4'>
          <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
            <Card>
              <CardHeader className='flex flex-row items-center justify-between py-0!'>
                <CardTitle className='text-sm font-medium'>
                  {t('ngoAdminDashboard.observersAccountsCardTitle')}
                </CardTitle>
                <Button
                  type='button'
                  variant='ghost'
                  onClick={() => {
                    saveChart(observersAccountsChartRef, 'observers-accounts.png');
                  }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <DoughnutChart
                  title={t('ngoAdminDashboard.observersAccountsIndicatorTitle')}
                  total={statistics?.observersStats?.totalNumberOfObservers ?? 0}
                  data={observersAccountsDataConfig(statistics?.observersStats)}
                  ref={observersAccountsChartRef}
                />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className='flex flex-row items-center justify-between py-0!'>
                <CardTitle className='text-sm font-medium'>
                  {t('ngoAdminDashboard.observersOnFieldCardTitle')}
                </CardTitle>
                <Button
                  type='button'
                  variant='ghost'
                  onClick={() => {
                    saveChart(observersOnFieldChartRef, 'observers-on-field.png');
                  }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <GaugeChart
                  title={t('ngoAdminDashboard.observersInPollingStationsIndicatorTitle')}
                  metricLabel={t('ngoAdminDashboard.observersInPollingStationsIndicatorMetricLabel')}
                  data={observersOnTheFieldDataConfig(
                    statistics?.observersStats?.totalNumberOfObservers,
                    statistics?.numberOfObserversOnTheField
                  )}
                  value={statistics?.numberOfObserversOnTheField ?? 0}
                  total={statistics?.observersStats?.totalNumberOfObservers ?? 0}
                  ref={observersOnFieldChartRef}
                />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-sm font-medium'>{t('ngoAdminDashboard.pollingStationCardTitle')}</CardTitle>
                <Button
                  type='button'
                  variant='ghost'
                  onClick={() => {
                    saveChart(pollingStationsChartRef, 'polling-stations-covered.png');
                  }}>
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
                  ref={pollingStationsChartRef}
                />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-sm font-medium'>
                  {t('ngoAdminDashboard.timeSpentObservingCardTitle')}
                </CardTitle>
                <Button
                  type='button'
                  variant='ghost'
                  onClick={() => {
                    saveChart(timeSpentObservingChartRef, 'time-spent-observing.png');
                  }}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <MetricChart
                  title={t('ngoAdminDashboard.timeSpentObservingIndicatorTitle')}
                  unit='h'
                  data={timeSpentObservingDataConfig(statistics?.minutesMonitoring)}
                  ref={timeSpentObservingChartRef}
                />
              </CardContent>
            </Card>
          </div>
          <div
            className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'
            style={{
              gridAutoFlow: expandedCharts.size > 0 ? 'row dense' : 'unset',
            }}>
            <Card
              className={cn('transition-all duration-300 ease-in-out', {
                'col-span-full': expandedCharts.has('startedFormsCard'),
              })}>
              <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-sm font-medium'>{t('ngoAdminDashboard.startedFormsCardTitle')}</CardTitle>
                <div>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChart(startedFormsChartRef, 'started-forms.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                  <Button type='button' variant='ghost' onClick={() => toggleChart('startedFormsCard')}>
                    {expandedCharts.has('startedFormsCard') ? (
                      <ArrowsPointingInIcon className='w-6 h-6 fill-gray-400' />
                    ) : (
                      <ArrowsPointingOutIcon className='w-6 h-6 fill-gray-400' />
                    )}
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <TimeLineChart
                  title={t('ngoAdminDashboard.startedFormsIndicatorTitle', {
                    interval: getInterval(statistics?.formsHistogram),
                  })}
                  data={histogramChartConfig(statistics?.formsHistogram)}
                  ref={startedFormsChartRef}
                  total={getTotal(statistics?.formsHistogram)}
                  showTotal
                />
              </CardContent>
            </Card>
            <Card
              className={cn('transition-all duration-300 ease-in-out', {
                'col-span-full': expandedCharts.has('questionsAnsweredCard'),
              })}>
              <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-sm font-medium'>
                  {t('ngoAdminDashboard.questionsAnsweredCardTitle')}
                </CardTitle>
                <div>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChart(questionsAnsweredChartRef, 'questions-answered.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                  <Button type='button' variant='ghost' onClick={() => toggleChart('questionsAnsweredCard')}>
                    {expandedCharts.has('questionsAnsweredCard') ? (
                      <ArrowsPointingInIcon className='w-6 h-6 fill-gray-400' />
                    ) : (
                      <ArrowsPointingOutIcon className='w-6 h-6 fill-gray-400' />
                    )}
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <TimeLineChart
                  title={t('ngoAdminDashboard.questionsAnsweredIndicatorTitle', {
                    interval: getInterval(statistics?.formsHistogram),
                  })}
                  data={histogramChartConfig(statistics?.questionsHistogram)}
                  ref={questionsAnsweredChartRef}
                  total={getTotal(statistics?.questionsHistogram)}
                  showTotal
                />
              </CardContent>
            </Card>
            <Card
              className={cn('transition-all duration-300 ease-in-out', {
                'col-span-full': expandedCharts.has('flaggedAnswersCard'),
              })}>
              <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-sm font-medium'>{t('ngoAdminDashboard.flaggedAnswersCardTitle')}</CardTitle>
                <div>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChart(flaggedAnswersChartRef, 'flagged-answers.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                  <Button type='button' variant='ghost' onClick={() => toggleChart('flaggedAnswersCard')}>
                    {expandedCharts.has('flaggedAnswersCard') ? (
                      <ArrowsPointingInIcon className='w-6 h-6 fill-gray-400' />
                    ) : (
                      <ArrowsPointingOutIcon className='w-6 h-6 fill-gray-400' />
                    )}
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <TimeLineChart
                  title={t('ngoAdminDashboard.flaggedAnswersIndicatorTitle', {
                    interval: getInterval(statistics?.formsHistogram),
                  })}
                  data={histogramChartConfig(statistics?.flaggedAnswersHistogram, 'red')}
                  ref={flaggedAnswersChartRef}
                  total={getTotal(statistics?.flaggedAnswersHistogram)}
                  showTotal
                />
              </CardContent>
            </Card>
            <Card
              className={cn('transition-all duration-300 ease-in-out', {
                'col-span-full': expandedCharts.has('quickReportsCard'),
              })}>
              <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-sm font-medium'>{t('ngoAdminDashboard.quickReportsCardTitle')}</CardTitle>
                <div>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChart(quickReportsChartRef, 'quick-reports.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                  <Button type='button' variant='ghost' onClick={() => toggleChart('quickReportsCard')}>
                    {expandedCharts.has('quickReportsCard') ? (
                      <ArrowsPointingInIcon className='w-6 h-6 fill-gray-400' />
                    ) : (
                      <ArrowsPointingOutIcon className='w-6 h-6 fill-gray-400' />
                    )}
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <TimeLineChart
                  title={t('ngoAdminDashboard.quickReportsIndicatorTitle', {
                    interval: getInterval(statistics?.quickReportsHistogram),
                  })}
                  data={histogramChartConfig(statistics?.quickReportsHistogram, 'red')}
                  ref={quickReportsChartRef}
                  total={getTotal(statistics?.quickReportsHistogram)}
                  showTotal
                />
              </CardContent>
            </Card>
            {isMonitoringNgoForCitizenReporting && (
              <Card
                className={cn('transition-all duration-300 ease-in-out', {
                  'col-span-full': expandedCharts.has('citizenReportsCard'),
                })}>
                <CardHeader className='flex flex-row items-center justify-between'>
                  <CardTitle className='text-sm font-medium'>
                    {t('ngoAdminDashboard.citizenReportsCardTitle')}
                  </CardTitle>
                  <div>
                    <Button
                      type='button'
                      variant='ghost'
                      onClick={() => {
                        saveChart(citizenReportsChartRef, 'quick-reports.png');
                      }}>
                      <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                    </Button>
                    <Button type='button' variant='ghost' onClick={() => toggleChart('citizenReportsCard')}>
                      {expandedCharts.has('citizenReportsCard') ? (
                        <ArrowsPointingInIcon className='w-6 h-6 fill-gray-400' />
                      ) : (
                        <ArrowsPointingOutIcon className='w-6 h-6 fill-gray-400' />
                      )}
                    </Button>
                  </div>
                </CardHeader>
                <CardContent>
                  <TimeLineChart
                    title={t('ngoAdminDashboard.citizenReportsIndicatorTitle', {
                      interval: getInterval(statistics?.citizenReportsHistogram),
                    })}
                    data={histogramChartConfig(statistics?.citizenReportsHistogram, 'red')}
                    ref={citizenReportsChartRef}
                    total={getTotal(statistics?.citizenReportsHistogram)}
                    showTotal
                  />
                </CardContent>
              </Card>
            )}
            <Card
              className={cn('transition-all duration-300 ease-in-out', {
                'col-span-full': expandedCharts.has('incidentReportsCard'),
              })}>
              <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-sm font-medium'>{t('ngoAdminDashboard.incidentReportsCardTitle')}</CardTitle>
                <div>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChart(incidentReportsChartRef, 'incident-reports.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                  <Button type='button' variant='ghost' onClick={() => toggleChart('incidentReportsCard')}>
                    {expandedCharts.has('incidentReportsCard') ? (
                      <ArrowsPointingInIcon className='w-6 h-6 fill-gray-400' />
                    ) : (
                      <ArrowsPointingOutIcon className='w-6 h-6 fill-gray-400' />
                    )}
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <TimeLineChart
                  title={t('ngoAdminDashboard.incidentReportsIndicatorTitle', {
                    interval: getInterval(statistics?.incidentReportsHistogram),
                  })}
                  data={histogramChartConfig(statistics?.incidentReportsHistogram, 'red')}
                  ref={incidentReportsChartRef}
                  total={getTotal(statistics?.incidentReportsHistogram)}
                  showTotal
                />
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </Layout>
  );
}
