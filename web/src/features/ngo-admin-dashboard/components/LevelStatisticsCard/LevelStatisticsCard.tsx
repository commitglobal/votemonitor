import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { convertToCSV, downloadCSV } from '@/lib/csv-helpers';
import { round, toKebabCase } from '@/lib/utils';
import { ArrowDownTrayIcon } from '@heroicons/react/24/solid';
import { orderBy } from 'lodash';
import { useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { VisitedPollingStationLevelStats } from '../../models/ngo-admin-statistics-models';

interface LevelStatsModel {
  path: string;
  value: number;
  unit?: string;
}

function LevelItem({ path, value, unit = '' }: LevelStatsModel) {
  return (
    <div className='flex items-center justify-between py-2 border-b last:border-b-0'>
      <span className='text-sm font-medium'>{path}</span>
      <span className='text-sm font-medium'>
        {value}
        {unit}
      </span>
    </div>
  );
}

interface LevelStatisticsCardProps {
  level: number;
  cardName: string;
  levelsStats: LevelStatsModel[];
}

function LevelStatisticsCard({ level, cardName, levelsStats }: LevelStatisticsCardProps) {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const data = useMemo(() => {
    return orderBy(levelsStats, [(l) => l.value, (l) => l.path], ['desc', 'asc']);
  }, [levelsStats]);


  function ExportDataButton() {
    return (
      <Button
        type='button'
        variant='ghost'
        onClick={() => {
          const csvData = convertToCSV(data);
          downloadCSV(csvData, toKebabCase(`level${level}-` + cardName + '.csv'));
        }}>
        <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
      </Button>
    );
  }

  return (
    <Card className='w-full'>
      <CardHeader className='flex flex-row items-center justify-between'>
        <CardTitle className='text-lg font-medium'>{cardName}</CardTitle>
        <ExportDataButton />
      </CardHeader>
      <CardContent>
        {data.length === 0 ? (
          <div className='flex flex-col items-center justify-center py-6 text-center text-muted-foreground'>
            <p>No data yet</p>
          </div>
        ) : null}
        {data.slice(0, 5).map((level) => (
          <LevelItem key={level.path} path={level.path} value={level.value} unit={level.unit} />
        ))}
        {data.length > 5 && (
          <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
            <DialogTrigger asChild>
              <Button variant='link' className='p-0 mt-2 text-purple-600'>
                View all
              </Button>
            </DialogTrigger>
            <DialogContent className='max-w-md max-h-[80vh] overflow-y-auto'>
              <DialogHeader className='flex flex-row items-center justify-between'>
                <DialogTitle>All levels</DialogTitle>
                <ExportDataButton />
              </DialogHeader>
              <div className='mt-4'>
                {data.map((level) => (
                  <LevelItem key={level.path} path={level.path} value={level.value} unit={level.unit} />
                ))}
              </div>
            </DialogContent>
          </Dialog>
        )}
      </CardContent>
    </Card>
  );
}

export interface LevelStatisticsProps {
  level: number;
  levelStats: VisitedPollingStationLevelStats[];
}

export default function LevelStatistics({ level, levelStats }: LevelStatisticsProps) {
  const { t } = useTranslation('translation', { keyPrefix: 'ngoAdminDashboard.pollingStationsLevelsCards' });

  const onFieldObserversData = useMemo(() => {
    return levelStats.map((s) => ({ path: s.path, value: s.activeObservers })).filter((x) => x.value > 0);
  }, [levelStats]);

  const questionsAnsweredData = useMemo(() => {
    return levelStats.map((s) => ({ path: s.path, value: s.numberOfQuestionsAnswered })).filter((x) => x.value > 0);
  }, [levelStats]);

  const flaggedAnswersData = useMemo(() => {
    return levelStats.map((s) => ({ path: s.path, value: s.numberOfFlaggedAnswers })).filter((x) => x.value > 0);
  }, [levelStats]);

  const visitedPollingStationsData = useMemo(() => {
    return levelStats
      .map((s) => ({ path: s.path, value: s.numberOfVisitedPollingStations }))
      .filter((x) => x.value > 0);
  }, [levelStats]);

  const pollingStationsCoverageData = useMemo(() => {
    return levelStats
      .map((s) => ({ path: s.path, value: round(s.coveragePercentage, 2), unit: ' %' }))
      .filter((x) => x.value > 0);
  }, [levelStats]);

  const timeSpentObservingData = useMemo(() => {
    return levelStats
      .map((s) => ({ path: s.path, value: round(s.minutesMonitoring / 60, 2), unit: ' h' }))
      .filter((x) => x.value > 0);
  }, [levelStats]);

  const quickReportsData = useMemo(() => {
    return levelStats.map((s) => ({ path: s.path, value: s.numberOfQuickReports })).filter((x) => x.value > 0);
  }, [levelStats]);

  const incidentReportsData = useMemo(() => {
    return levelStats.map((s) => ({ path: s.path, value: s.numberOfIncidentReports })).filter((x) => x.value > 0);
  }, [levelStats]);

  return (
    <div className='container p-4 mx-auto'>
      <div className='grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4'>
        <LevelStatisticsCard cardName={t('onFieldObservers')} levelsStats={onFieldObserversData} level={level} />
        <LevelStatisticsCard cardName={t('questionsAnswered')} levelsStats={questionsAnsweredData} level={level} />
        <LevelStatisticsCard cardName={t('flaggedAnswers')} levelsStats={flaggedAnswersData} level={level} />
        <LevelStatisticsCard
          cardName={t('visitedPollingStations')}
          levelsStats={visitedPollingStationsData}
          level={level}
        />
        <LevelStatisticsCard
          cardName={t('pollingStationsCoverage')}
          levelsStats={pollingStationsCoverageData}
          level={level}
        />
        <LevelStatisticsCard cardName={t('timeSpentObserving')} levelsStats={timeSpentObservingData} level={level} />
        <LevelStatisticsCard cardName={t('quickReports')} levelsStats={quickReportsData} level={level} />
        {/* <LevelStatisticsCard cardName={t('incidentReports')} levelsStats={incidentReportsData} level={level} /> */}
      </div>
    </div>
  );
}
