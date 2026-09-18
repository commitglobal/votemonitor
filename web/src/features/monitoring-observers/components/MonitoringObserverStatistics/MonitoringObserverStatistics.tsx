import type { FunctionComponent } from '@/common/types';
import { useDataSource } from '@/common/data-source-store';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { convertToCSV, downloadCSV } from '@/lib/csv-helpers';
import { cn, round, toKebabCase } from '@/lib/utils';
import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';
import {
  BoltIcon,
  ClockIcon,
  DocumentTextIcon,
  ExclamationTriangleIcon,
  FlagIcon,
  MapPinIcon,
} from '@heroicons/react/24/outline';
import { ArrowDownTrayIcon } from '@heroicons/react/24/solid';
import { orderBy } from 'lodash';
import { useMemo, type ReactNode } from 'react';
import type { VisitedPollingStationLevelStats } from '@/features/ngo-admin-dashboard/models/ngo-admin-statistics-models';
import { useMonitoringObserverStatistics } from '../../hooks/monitoring-observers-queries';

type BarColor = 'amber' | 'red' | 'green';

interface LocationMetric {
  path: string;
  value: number;
  displayValue: string;
}

interface LevelMetricCardProps {
  title: string;
  level: number;
  metrics: LocationMetric[];
  barColor: BarColor;
  formatTotal?: (total: number) => string;
}

const barColorClass: Record<BarColor, string> = {
  amber: 'bg-amber-400',
  red: 'bg-red-400',
  green: 'bg-emerald-500',
};

function leafPath(path: string): string {
  const parts = path.split(' / ');
  return parts[parts.length - 1] || path;
}

function LevelMetricCard({ title, level, metrics, barColor, formatTotal }: LevelMetricCardProps) {
  const data = useMemo(
    () => orderBy(metrics, [(m) => m.value, (m) => m.path], ['desc', 'asc']),
    [metrics]
  );
  const total = data.reduce((sum, m) => sum + m.value, 0);
  const max = Math.max(...data.map((m) => m.value), 1);
  const totalLabel = formatTotal ? formatTotal(total) : String(total);
  const locationCount = data.length;

  return (
    <Card className='w-full'>
      <CardHeader className='flex flex-row items-start justify-between space-y-0 pb-2'>
        <div>
          <CardTitle className='text-base font-medium text-gray-700'>{title}</CardTitle>
          <div className='mt-2 text-3xl font-semibold tracking-tight text-gray-900'>{totalLabel}</div>
          <p className='mt-1 text-sm text-muted-foreground'>
            {locationCount === 0
              ? 'No locations'
              : `across ${locationCount} location${locationCount === 1 ? '' : 's'}`}
          </p>
        </div>
        <Button
          type='button'
          variant='ghost'
          size='icon'
          disabled={data.length === 0}
          onClick={() => {
            const csvData = convertToCSV(
              data.map((m) => ({ path: m.path, value: m.displayValue }))
            );
            downloadCSV(csvData, toKebabCase(`level${level}-${title}.csv`));
          }}>
          <ArrowDownTrayIcon className='h-5 w-5 fill-gray-400' />
        </Button>
      </CardHeader>
      <CardContent>
        {data.length === 0 ? (
          <div className='flex items-center justify-center py-8 text-sm text-muted-foreground'>No data yet</div>
        ) : (
          <ul className='space-y-3'>
            {data.map((metric) => (
              <li key={metric.path} className='space-y-1'>
                <div className='flex items-center justify-between gap-3 text-sm'>
                  <span className='truncate font-medium text-gray-800'>{leafPath(metric.path)}</span>
                  <span className='shrink-0 text-gray-600'>{metric.displayValue}</span>
                </div>
                <div className='h-2 overflow-hidden rounded-full bg-gray-100'>
                  <div
                    className={cn('h-full rounded-full', barColorClass[barColor])}
                    style={{ width: `${(metric.value / max) * 100}%` }}
                  />
                </div>
              </li>
            ))}
          </ul>
        )}
      </CardContent>
    </Card>
  );
}

function ByLocationLevel({ level, levelStats }: { level: number; levelStats: VisitedPollingStationLevelStats[] }) {
  const questionsAnswered = useMemo(
    () =>
      levelStats
        .filter((s) => s.numberOfQuestionsAnswered > 0)
        .map((s) => ({
          path: s.path,
          value: s.numberOfQuestionsAnswered,
          displayValue: String(s.numberOfQuestionsAnswered),
        })),
    [levelStats]
  );

  const flaggedAnswers = useMemo(
    () =>
      levelStats
        .filter((s) => s.numberOfFlaggedAnswers > 0)
        .map((s) => ({
          path: s.path,
          value: s.numberOfFlaggedAnswers,
          displayValue: String(s.numberOfFlaggedAnswers),
        })),
    [levelStats]
  );

  const visitedPollingStations = useMemo(
    () =>
      levelStats
        .filter((s) => s.numberOfVisitedPollingStations > 0)
        .map((s) => ({
          path: s.path,
          value: s.numberOfVisitedPollingStations,
          displayValue: String(s.numberOfVisitedPollingStations),
        })),
    [levelStats]
  );

  const timeSpent = useMemo(
    () =>
      levelStats
        .filter((s) => s.minutesMonitoring > 0)
        .map((s) => ({
          path: s.path,
          value: s.minutesMonitoring,
          displayValue: `${round(s.minutesMonitoring / 60, 2)} h`,
        })),
    [levelStats]
  );

  const quickReports = useMemo(
    () =>
      levelStats
        .filter((s) => s.numberOfQuickReports > 0)
        .map((s) => ({
          path: s.path,
          value: s.numberOfQuickReports,
          displayValue: String(s.numberOfQuickReports),
        })),
    [levelStats]
  );

  const incidentReports = useMemo(
    () =>
      levelStats
        .filter((s) => s.numberOfIncidentReports > 0)
        .map((s) => ({
          path: s.path,
          value: s.numberOfIncidentReports,
          displayValue: String(s.numberOfIncidentReports),
        })),
    [levelStats]
  );

  return (
    <div className='grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3'>
      <LevelMetricCard title='Questions answered' level={level} metrics={questionsAnswered} barColor='amber' />
      <LevelMetricCard title='Flagged answers' level={level} metrics={flaggedAnswers} barColor='red' />
      <LevelMetricCard title='Visited polling stations' level={level} metrics={visitedPollingStations} barColor='green' />
      <LevelMetricCard
        title='Time spent observing'
        level={level}
        metrics={timeSpent}
        barColor='amber'
        formatTotal={(total) => `${round(total / 60, 2)} h`}
      />
      <LevelMetricCard title='Quick reports' level={level} metrics={quickReports} barColor='red' />
      <LevelMetricCard title='Incident reports' level={level} metrics={incidentReports} barColor='red' />
    </div>
  );
}

interface WorkloadCardProps {
  title: string;
  value: string;
  icon: ReactNode;
}

function WorkloadCard({ title, value, icon }: WorkloadCardProps) {
  return (
    <Card>
      <CardContent className='flex items-start gap-3 p-4'>
        <div className='rounded-md bg-gray-100 p-2 text-gray-600'>{icon}</div>
        <div className='min-w-0'>
          <p className='text-sm text-muted-foreground'>{title}</p>
          <p className='mt-1 text-2xl font-semibold tracking-tight text-gray-900'>{value}</p>
        </div>
      </CardContent>
    </Card>
  );
}

export default function MonitoringObserverStatistics(): FunctionComponent {
  const { monitoringObserverId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const dataSource = useDataSource();
  const { data: statistics } = useMonitoringObserverStatistics(
    currentElectionRoundId,
    monitoringObserverId,
    dataSource
  );

  const timeSpentHours = round((statistics?.minutesMonitoring ?? 0) / 60, 2);

  return (
    <div className='flex w-full flex-col gap-8'>
      <section className='space-y-3'>
        <h2 className='text-xl font-semibold text-gray-900'>Workload</h2>
        <div className='grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6'>
          <WorkloadCard
            title='Visited polling stations'
            value={String(statistics?.numberOfPollingStationsVisited ?? 0)}
            icon={<MapPinIcon className='h-5 w-5' />}
          />
          <WorkloadCard
            title='Form submissions'
            value={String(statistics?.numberOfFormsSubmitted ?? 0)}
            icon={<DocumentTextIcon className='h-5 w-5' />}
          />
          <WorkloadCard
            title='Flagged answers'
            value={`${statistics?.numberOfFlaggedAnswers ?? 0} / ${statistics?.numberOfQuestionsAnswered ?? 0}`}
            icon={<FlagIcon className='h-5 w-5' />}
          />
          <WorkloadCard
            title='Time spent monitoring'
            value={`${timeSpentHours} h`}
            icon={<ClockIcon className='h-5 w-5' />}
          />
          <WorkloadCard
            title='Quick reports'
            value={String(statistics?.numberOfQuickReports ?? 0)}
            icon={<BoltIcon className='h-5 w-5' />}
          />
          <WorkloadCard
            title='Incident reports'
            value={String(statistics?.numberOfIncidentReports ?? 0)}
            icon={<ExclamationTriangleIcon className='h-5 w-5' />}
          />
        </div>
      </section>

      <section className='space-y-3'>
        <div>
          <h2 className='text-xl font-semibold text-gray-900'>By location</h2>
          <p className='text-sm text-muted-foreground'>
            This observer&apos;s activity broken down by administrative level.
          </p>
        </div>

        <Tabs defaultValue='level-1'>
          <TabsList
            className={cn('grid bg-slate-200', {
              'grid-cols-1 w-[100px]':
                !statistics?.level2Stats?.length &&
                !statistics?.level3Stats?.length &&
                !statistics?.level4Stats?.length &&
                !statistics?.level5Stats?.length,
              'grid-cols-2 w-[200px]':
                statistics?.level2Stats?.length &&
                !statistics?.level3Stats?.length &&
                !statistics?.level4Stats?.length &&
                !statistics?.level5Stats?.length,
              'grid-cols-3 w-[300px]':
                statistics?.level3Stats?.length &&
                !statistics?.level4Stats?.length &&
                !statistics?.level5Stats?.length,
              'grid-cols-4 w-[400px]': statistics?.level4Stats?.length && !statistics?.level5Stats?.length,
              'grid-cols-5 w-[500px]': !!statistics?.level5Stats?.length,
            })}>
            <TabsTrigger value='level-1'>Level 1</TabsTrigger>
            {statistics?.level2Stats?.length ? <TabsTrigger value='level-2'>Level 2</TabsTrigger> : null}
            {statistics?.level3Stats?.length ? <TabsTrigger value='level-3'>Level 3</TabsTrigger> : null}
            {statistics?.level4Stats?.length ? <TabsTrigger value='level-4'>Level 4</TabsTrigger> : null}
            {statistics?.level5Stats?.length ? <TabsTrigger value='level-5'>Level 5</TabsTrigger> : null}
          </TabsList>

          <TabsContent value='level-1' className='mt-4'>
            <ByLocationLevel level={1} levelStats={statistics?.level1Stats ?? []} />
          </TabsContent>
          {statistics?.level2Stats?.length ? (
            <TabsContent value='level-2' className='mt-4'>
              <ByLocationLevel level={2} levelStats={statistics.level2Stats} />
            </TabsContent>
          ) : null}
          {statistics?.level3Stats?.length ? (
            <TabsContent value='level-3' className='mt-4'>
              <ByLocationLevel level={3} levelStats={statistics.level3Stats} />
            </TabsContent>
          ) : null}
          {statistics?.level4Stats?.length ? (
            <TabsContent value='level-4' className='mt-4'>
              <ByLocationLevel level={4} levelStats={statistics.level4Stats} />
            </TabsContent>
          ) : null}
          {statistics?.level5Stats?.length ? (
            <TabsContent value='level-5' className='mt-4'>
              <ByLocationLevel level={5} levelStats={statistics.level5Stats} />
            </TabsContent>
          ) : null}
        </Tabs>
      </section>
    </div>
  );
}
