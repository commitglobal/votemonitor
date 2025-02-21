import DoughnutChart from '@/components/charts/doughnut-chart/DoughnutChart';
import GaugeChart from '@/components/charts/gauge-chart/GaugeChart';
import MetricChart from '@/components/charts/metric-chart/MetricChart';
import TimeLineChart from '@/components/charts/time-line-chart/TimeLineChart';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ArrowDownTrayIcon, ArrowsPointingInIcon, ArrowsPointingOutIcon } from '@heroicons/react/24/solid';
import { useCallback, useRef } from 'react';

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
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { useTranslation } from 'react-i18next';
import { useElectionRoundStatistics } from '../../hooks/statistics-queries';
import { HistogramEntry } from '../../models/ngo-admin-statistics-models';
import LevelStatistics from '../LevelStatisticsCard/LevelStatisticsCard';
import useDashboardExpandedChartsStore from './dashboard-config.store';
import { DataSourceSwitcher } from '@/components/DataSourceSwitcher/DataSourceSwitcher';
import { useDataSource } from '@/common/data-source-store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';

export default function NgoAdminDashboard(): FunctionComponent {
  const { t } = useTranslation('translation', { keyPrefix: 'ngoAdminDashboard' });
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
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);
  const dataSource = useDataSource();

  const { data: statistics } = useElectionRoundStatistics(currentElectionRoundId, dataSource);

  const getInterval = useCallback((histogram: HistogramEntry[] | undefined) => {
    if (histogram && histogram.some((x) => x)) {
      const data = histogram.map((x) => new Date(x.bucket).getTime());
      const minDate = new Date(Math.min(...data));
      const maxDate = new Date(Math.max(...data));
      return `${format(minDate, DateTimeHourBucketFormat)} - ${format(maxDate, DateTimeHourBucketFormat)}`;
    }
    return '-';
  }, []);

  const getTotal = useCallback((formsHistogram: HistogramEntry[] | undefined) => {
    return (formsHistogram ?? []).reduce((acc, { value }) => acc + value, 0);
  }, []);

  const saveChartCallback = useCallback((chartRef: any, fileName: string) => {
    if (chartRef?.current) {
      saveChart(chartRef, fileName);
    }
  }, []);

  const getHistogramChartConfig = useCallback(
    (histogram: HistogramEntry[] | undefined, variant: 'red' | 'blue' = 'blue') => {
      return histogramChartConfig(histogram, variant);
    },
    []
  );

  const observersOnTheFieldData = useCallback(
    (totalNumberOfObservers: number | undefined, numberOfObserversOnTheField?: number) =>
      observersOnTheFieldDataConfig(totalNumberOfObservers, numberOfObserversOnTheField ?? 0),
    []
  );

  return (
    <>
      <header className='container py-4'>
        <div className='flex flex-col gap-1 text-gray-400'>
          <h1 className='flex flex-row items-center gap-3 text-3xl font-bold tracking-tight text-gray-900'>
            {t('title')}
          </h1>
          <div className='flex flex-row w-ful justify-between'>
            <h3 className='text-lg font-light'>{t('subtitle')}</h3>
            <DataSourceSwitcher />
          </div>
        </div>
      </header>
      <main className='container flex flex-col flex-1'>
        <div className='flex-col md:flex'>
          <div className='flex-1 space-y-4'>
            <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
              <Card>
                <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                  <CardTitle className='text-sm font-medium'>{t('observersAccounts.cardTitle')}</CardTitle>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChartCallback(observersAccountsChartRef, 'observers-accounts.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                </CardHeader>
                <CardContent>
                  <DoughnutChart
                    title={t('observersAccounts.indicatorTitle')}
                    total={statistics?.observersStats?.totalNumberOfObservers ?? 0}
                    data={observersAccountsDataConfig(statistics?.observersStats)}
                    ref={observersAccountsChartRef}
                  />
                </CardContent>
              </Card>
              <Card>
                <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                  <CardTitle className='text-sm font-medium'>{t('observersOnFieldCardTitle')}</CardTitle>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChartCallback(observersOnFieldChartRef, 'observers-on-field.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                </CardHeader>
                <CardContent>
                  <GaugeChart
                    title={t('observersInPollingStations.indicatorTitle')}
                    metricLabel={t('observersInPollingStations.metricLabel')}
                    data={observersOnTheFieldData(
                      statistics?.observersStats?.totalNumberOfObservers,
                      statistics?.totalStats?.activeObservers ?? 0
                    )}
                    value={statistics?.totalStats?.activeObservers ?? 0}
                    total={statistics?.observersStats?.totalNumberOfObservers ?? 0}
                    ref={observersOnFieldChartRef}
                  />
                </CardContent>
              </Card>
              <Card>
                <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                  <CardTitle className='text-sm font-medium'>{t('pollingStationCardTitle')}</CardTitle>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChartCallback(pollingStationsChartRef, 'polling-stations-covered.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                </CardHeader>
                <CardContent>
                  <GaugeChart
                    title={t('stationsVisitedByAtLeastOneObserver.indicatorTitle')}
                    metricLabel={t('stationsVisitedByAtLeastOneObserver.metricLabel')}
                    data={pollingStationsDataConfig(statistics?.totalStats)}
                    total={statistics?.totalStats?.numberOfPollingStations ?? 0}
                    value={statistics?.totalStats?.numberOfVisitedPollingStations ?? 0}
                    ref={pollingStationsChartRef}
                  />
                </CardContent>
              </Card>
              <Card>
                <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                  <CardTitle className='text-sm font-medium'>{t('timeSpentObserving.cardTitle')}</CardTitle>
                  <Button
                    type='button'
                    variant='ghost'
                    onClick={() => {
                      saveChartCallback(timeSpentObservingChartRef, 'time-spent-observing.png');
                    }}>
                    <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                  </Button>
                </CardHeader>
                <CardContent>
                  <MetricChart
                    title={t('timeSpentObserving.indicatorTitle')}
                    unit='h'
                    data={timeSpentObservingDataConfig(statistics?.totalStats?.minutesMonitoring ?? 0)}
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
                <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                  <CardTitle className='text-sm font-medium'>{t('startedForms.cardTitle')}</CardTitle>
                  <div>
                    <Button
                      type='button'
                      variant='ghost'
                      onClick={() => {
                        saveChartCallback(startedFormsChartRef, 'started-forms.png');
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
                    title={t('startedForms.indicatorTitle', {
                      interval: getInterval(statistics?.formsHistogram),
                    })}
                    data={getHistogramChartConfig(statistics?.formsHistogram)}
                    ref={startedFormsChartRef}
                    total={getTotal(statistics?.formsHistogram)}
                  />
                </CardContent>
              </Card>
              <Card
                className={cn('transition-all duration-300 ease-in-out', {
                  'col-span-full': expandedCharts.has('questionsAnsweredCard'),
                })}>
                <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                  <CardTitle className='text-sm font-medium'>{t('questionsAnswered.cardTitle')}</CardTitle>
                  <div>
                    <Button
                      type='button'
                      variant='ghost'
                      onClick={() => {
                        saveChartCallback(questionsAnsweredChartRef, 'questions-answered.png');
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
                    title={t('questionsAnswered.indicatorTitle', {
                      interval: getInterval(statistics?.questionsHistogram),
                    })}
                    data={getHistogramChartConfig(statistics?.questionsHistogram)}
                    ref={questionsAnsweredChartRef}
                    total={getTotal(statistics?.questionsHistogram)}
                  />
                </CardContent>
              </Card>
              <Card
                className={cn('transition-all duration-300 ease-in-out', {
                  'col-span-full': expandedCharts.has('flaggedAnswersCard'),
                })}>
                <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                  <CardTitle className='text-sm font-medium'>{t('flaggedAnswers.cardTitle')}</CardTitle>
                  <div>
                    <Button
                      type='button'
                      variant='ghost'
                      onClick={() => {
                        saveChartCallback(flaggedAnswersChartRef, 'flagged-answers.png');
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
                    title={t('flaggedAnswers.indicatorTitle', {
                      interval: getInterval(statistics?.flaggedAnswersHistogram),
                    })}
                    data={getHistogramChartConfig(statistics?.flaggedAnswersHistogram, 'red')}
                    ref={flaggedAnswersChartRef}
                    total={getTotal(statistics?.flaggedAnswersHistogram)}
                  />
                </CardContent>
              </Card>
              <Card
                className={cn('transition-all duration-300 ease-in-out', {
                  'col-span-full': expandedCharts.has('quickReportsCard'),
                })}>
                <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                  <CardTitle className='text-sm font-medium'>{t('quickReports.cardTitle')}</CardTitle>
                  <div>
                    <Button
                      type='button'
                      variant='ghost'
                      onClick={() => {
                        saveChartCallback(quickReportsChartRef, 'quick-reports.png');
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
                    title={t('quickReports.indicatorTitle', {
                      interval: getInterval(statistics?.quickReportsHistogram),
                    })}
                    data={getHistogramChartConfig(statistics?.quickReportsHistogram, 'red')}
                    ref={quickReportsChartRef}
                    total={getTotal(statistics?.quickReportsHistogram)}
                  />
                </CardContent>
              </Card>
              {electionRound?.isMonitoringNgoForCitizenReporting && (
                <Card
                  className={cn('transition-all duration-300 ease-in-out', {
                    'col-span-full': expandedCharts.has('citizenReportsCard'),
                  })}>
                  <CardHeader className='flex flex-row items-center justify-between space-y-0'>
                    <CardTitle className='text-sm font-medium'>{t('citizenReports.cardTitle')}</CardTitle>
                    <div>
                      <Button
                        type='button'
                        variant='ghost'
                        onClick={() => {
                          saveChartCallback(citizenReportsChartRef, 'quick-reports.png');
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
                      title={t('citizenReports.indicatorTitle', {
                        interval: getInterval(statistics?.citizenReportsHistogram),
                      })}
                      data={getHistogramChartConfig(statistics?.citizenReportsHistogram, 'red')}
                      ref={citizenReportsChartRef}
                      total={getTotal(statistics?.citizenReportsHistogram)}
                    />
                  </CardContent>
                </Card>
              )}
              {/* <Card
                className={cn('transition-all duration-300 ease-in-out', {
                  'col-span-full': expandedCharts.has('incidentReportsCard'),
                })}>
                <CardHeader className='flex flex-row items-center justify-between'>
                  <CardTitle className='text-sm font-medium'>{t('incidentReports.cardTitle')}</CardTitle>
                  <div>
                    <Button
                      type='button'
                      variant='ghost'
                      onClick={() => {
                        saveChartCallback(incidentReportsChartRef, 'incident-reports.png');
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
                    title={t('incidentReports.indicatorTitle', {
                      interval: getInterval(statistics?.incidentReportsHistogram),
                    })}
                    data={getHistogramChartConfig(statistics?.incidentReportsHistogram, 'red')}
                    ref={incidentReportsChartRef}
                    total={getTotal(statistics?.incidentReportsHistogram)}
                  />
                </CardContent>
              </Card> */}
            </div>
            <div>
              <Tabs defaultValue='level-1'>
                <TabsList
                  className={cn('grid bg-slate-200', {
                    'grid-cols-1 w-[100px]':
                      statistics?.level2Stats?.length === 0 &&
                      statistics?.level3Stats?.length === 0 &&
                      statistics?.level4Stats?.length === 0 &&
                      statistics?.level5Stats?.length === 0,
                    'grid-cols-2 w-[200px]':
                      statistics?.level2Stats?.length &&
                      statistics?.level3Stats?.length === 0 &&
                      statistics?.level4Stats?.length === 0 &&
                      statistics?.level5Stats?.length === 0,
                    'grid-cols-3 w-[300px]':
                      statistics?.level3Stats?.length &&
                      statistics?.level4Stats?.length === 0 &&
                      statistics?.level5Stats?.length === 0,
                    'grid-cols-4 w-[400px]': statistics?.level4Stats?.length && statistics?.level5Stats?.length === 0,
                    'grid-cols-5 w-[500px]': statistics?.level5Stats?.length,
                  })}>
                  <TabsTrigger value='level-1'>Level 1</TabsTrigger>
                  {statistics?.level2Stats?.length ? <TabsTrigger value='level-2'>Level 2</TabsTrigger> : null}
                  {statistics?.level3Stats?.length ? <TabsTrigger value='level-3'>Level 3</TabsTrigger> : null}
                  {statistics?.level4Stats?.length ? <TabsTrigger value='level-4'>Level 4</TabsTrigger> : null}
                  {statistics?.level5Stats?.length ? <TabsTrigger value='level-5'>Level 5</TabsTrigger> : null}
                </TabsList>

                <TabsContent value='level-1'>
                  <LevelStatistics level={1} levelStats={statistics?.level1Stats ?? []} />
                </TabsContent>

                {statistics?.level2Stats?.length ? (
                  <TabsContent value='level-2'>
                    <LevelStatistics level={2} levelStats={statistics?.level2Stats ?? []} />
                  </TabsContent>
                ) : null}

                {statistics?.level3Stats?.length ? (
                  <TabsContent value='level-3'>
                    <LevelStatistics level={3} levelStats={statistics?.level3Stats ?? []} />
                  </TabsContent>
                ) : null}

                {statistics?.level4Stats?.length ? (
                  <TabsContent value='level-4'>
                    <LevelStatistics level={4} levelStats={statistics?.level4Stats ?? []} />
                  </TabsContent>
                ) : null}

                {statistics?.level5Stats?.length ? (
                  <TabsContent value='level-5'>
                    <LevelStatistics level={5} levelStats={statistics?.level5Stats ?? []} />
                  </TabsContent>
                ) : null}
              </Tabs>
            </div>
          </div>
        </div>
      </main>
    </>
  );
}
