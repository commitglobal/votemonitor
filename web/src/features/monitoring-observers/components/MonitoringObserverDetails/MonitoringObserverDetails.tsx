import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

import MonitoringObserverDetailsView from '../MonitoringObserverDetailsView/MonitoringObserverDetailsView';
import { MonitoringObserverFormSubmissions } from '../MonitoringObserverFormSubmissions/MonitoringObserverFormSubmissions';

import type { FunctionComponent } from '@/common/types';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { monitoringObserverDetailsQueryOptions } from '@/routes/monitoring-observers/edit.$monitoringObserverId';
import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { MonitorObserverBackButton } from '../MonitoringObserverBackButton';
import { MonitoringObserverQuickReports } from '../MonitoringObserverQuickReports/MonitoringObserverQuickReports';

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
      title={`${monitoringObserver.displayName}`}>
      <Tabs defaultValue='details' value={currentTab} onValueChange={handleTabChange}>
        <TabsList className='grid grid-cols-3 bg-gray-200 w-[600px] mb-4'>
          <TabsTrigger value='details'>Observer details</TabsTrigger>
          <TabsTrigger value='responses'>Form responses</TabsTrigger>
          <TabsTrigger value='quick-reports'>Quick reports</TabsTrigger>
          {/* <TabsTrigger value='incident-reports'>Incident reports</TabsTrigger> */}
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
        {/* <TabsContent value='incident-reports'>
          <MonitoringObserverIncidentReports />
        </TabsContent> */}
      </Tabs>
    </Layout>
  );
}
