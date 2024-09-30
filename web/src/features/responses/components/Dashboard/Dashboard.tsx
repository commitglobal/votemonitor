import { useSetPrevSearch } from '@/common/prev-search-store';
import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getRouteApi } from '@tanstack/react-router';
import { type ReactElement } from 'react';
import { CitizenReportsTab } from '../CitizenReportsTab/CitizenReportsTab';
import FormSubmissionsTab from '../FormSubmissionsTab/FormSubmissionsTab';
import { QuickReportsTab } from '../QuickReportsTab/QuickReportsTab';
import { cn } from '@/lib/utils';
import IssueReportsTab from '../IssueReportsTab/IssueReportsTab';
import { useTranslation } from 'react-i18next';

const routeApi = getRouteApi('/responses/');

export default function ResponsesDashboard(): ReactElement {
  const isMonitoringNgoForCitizenReporting = useCurrentElectionRoundStore((s) => s.isMonitoringNgoForCitizenReporting);
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const { tab } = search;
  const { t } = useTranslation();

  const setPrevSearch = useSetPrevSearch();

  return (
    <Layout title='Responses' subtitle='View all form answers and other issues reported by your observers.'>
      <Tabs
        defaultValue={tab ?? 'form-answers'}
        onValueChange={(tab) => {
          void navigate({
            search() {
              const newSearch = { tab };
              setPrevSearch(newSearch);
              return newSearch;
            },
          });
        }}>
        <TabsList
          className={cn('grid bg-gray-200 mb-4 grid-cols-3 w-[600px]', {
            'grid-cols-4 w-[800px]': isMonitoringNgoForCitizenReporting,
          })}>
          <TabsTrigger value='form-answers'>Form answers</TabsTrigger>
          <TabsTrigger value='quick-reports'>Quick reports</TabsTrigger>
          {isMonitoringNgoForCitizenReporting && <TabsTrigger value='citizen-reports'>Citizen reports</TabsTrigger>}
          <TabsTrigger value='issue-reports'>Issue reports</TabsTrigger>
        </TabsList>

        <TabsContent value='form-answers'>
          <FormSubmissionsTab />
        </TabsContent>

        <TabsContent value='quick-reports'>
          <QuickReportsTab />
        </TabsContent>

        <TabsContent value='issue-reports'>
          <IssueReportsTab />
        </TabsContent>

        {isMonitoringNgoForCitizenReporting && (
          <TabsContent value='citizen-reports'>
            <CitizenReportsTab />
          </TabsContent>
        )}
      </Tabs>
    </Layout>
  );
}
