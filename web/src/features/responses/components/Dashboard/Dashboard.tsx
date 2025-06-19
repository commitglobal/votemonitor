import { useSetPrevSearch } from '@/common/prev-search-store';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { cn } from '@/lib/utils';
import { Route } from '@/routes/(app)/responses';
import { type ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { CitizenReportsTab } from '../CitizenReportsTab/CitizenReportsTab';
import FormSubmissionsTab from '../FormSubmissionsTab/FormSubmissionsTab';
import IncidentReportsTab from '../IncidentReportsTab/IncidentReportsTab';
import { QuickReportsTab } from '../QuickReportsTab/QuickReportsTab';
import { useNavigate } from '@tanstack/react-router';
import { DataSourceSwitcher } from '@/components/DataSourceSwitcher/DataSourceSwitcher';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';

export default function ResponsesDashboard(): ReactElement {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);

  const navigate = useNavigate();
  const search = Route.useSearch();
  const { tab } = search;
  const { t } = useTranslation('translation', { keyPrefix: 'responses' });

  const setPrevSearch = useSetPrevSearch();

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
        <Tabs
          defaultValue={tab ?? 'form-answers'}
          onValueChange={(tab) => {
            navigate({
              to: '.',
              search() {
                const newSearch = { tab };
                setPrevSearch(newSearch);
                return newSearch;
              },
              replace: true,
            });
          }}>
          <TabsList
            className={cn('grid bg-gray-200 mb-4 grid-cols-2 w-[400px]', {
              'grid-cols-3 w-[600px]': electionRound?.isMonitoringNgoForCitizenReporting,
            })}>
            <TabsTrigger value='form-answers'>Form answers</TabsTrigger>
            <TabsTrigger value='quick-reports'>Quick reports</TabsTrigger>
            {electionRound?.isMonitoringNgoForCitizenReporting && (
              <TabsTrigger value='citizen-reports'>Citizen reports</TabsTrigger>
            )}
            {/* <TabsTrigger value='incident-reports'>Incident reports</TabsTrigger> */}
          </TabsList>

          <TabsContent value='form-answers'>
            <FormSubmissionsTab />
          </TabsContent>

          <TabsContent value='quick-reports'>
            <QuickReportsTab />
          </TabsContent>

          {/* <TabsContent value='incident-reports'>
            <IncidentReportsTab />
          </TabsContent> */}

          {electionRound?.isMonitoringNgoForCitizenReporting && (
            <TabsContent value='citizen-reports'>
              <CitizenReportsTab />
            </TabsContent>
          )}
        </Tabs>
      </main>
    </>
  );
}
