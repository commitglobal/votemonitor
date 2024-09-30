import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

import MonitoringObserverDetailsView from '../MonitoringObserverDetailsView/MonitoringObserverDetailsView';
import { MonitoringObserverFormSubmissions } from '../MonitoringObserverFormSubmissions/MonitoringObserverFormSubmissions';

import { monitoringObserverDetailsQueryOptions } from '@/common/queryOptions';
import type { FunctionComponent } from '@/common/types';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';
import { useSuspenseQuery } from '@tanstack/react-query';
import { MonitorObserverBackButton } from '../MonitoringObserverBackButton';
import { MonitoringObserverQuickReports } from '../MonitoringObserverQuickReports/MonitoringObserverQuickReports';
import { useState } from 'react';
import { useNavigate } from '@tanstack/react-router';
import { MonitoringObserverIssueReports } from '../MonitoringObserverIssueReports/MonitoringObserverIssueReports';

export default function MonitoringObserverDetails(): FunctionComponent {
  const { monitoringObserverId, tab } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const monitoringObserverQuery = useSuspenseQuery(
    monitoringObserverDetailsQueryOptions(currentElectionRoundId, monitoringObserverId)
  );
  const monitoringObserver = monitoringObserverQuery.data;

  const [currentTab, setCurrentTab] = useState(tab);
  const navigate = useNavigate();

  function handleTabChange(tab: string): void {
    setCurrentTab(tab);
    navigate({
      params(prev) {
        return { ...prev, tab };
      },
    });
  }
  return (
    <Layout
      backButton={<MonitorObserverBackButton />}
      title={`${monitoringObserver.firstName} ${monitoringObserver.lastName}`}>
      <Tabs defaultValue='details' value={currentTab} onValueChange={handleTabChange}>
        <TabsList className='grid grid-cols-4 bg-gray-200 w-[800px] mb-4'>
          <TabsTrigger value='details'>Observer details</TabsTrigger>
          <TabsTrigger value='responses'>Form responses</TabsTrigger>
          <TabsTrigger value='quick-reports'>Quick reports</TabsTrigger>
          <TabsTrigger value='issue-reports'>Issue reports</TabsTrigger>
        </TabsList>
        <TabsContent value='details'>
          <MonitoringObserverDetailsView />
        </TabsContent>
        <TabsContent value='responses'>
          <MonitoringObserverFormSubmissions />
        </TabsContent>
        <TabsContent value='quick-reports'>
          <MonitoringObserverQuickReports />
        </TabsContent>
        <TabsContent value='issue-reports'>
          <MonitoringObserverIssueReports />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
