import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useLoaderData, useNavigate } from '@tanstack/react-router';

import MonitoringObserverDetailsView from '../MonitoringObserverDetailsView/MonitoringObserverDetailsView';
import { MonitoringObserverForms } from '../MonitoringObserverForms/MonitoringObserverForms';

import type { FunctionComponent } from '@/common/types';
import type { MonitoringObserver } from '../../models/monitoring-observer';
import { Route } from '@/routes/monitoring-observers/$monitoringObserverId.view.$tab';
import { useState } from 'react';
export default function MonitoringObserverDetails(): FunctionComponent {
  const monitoringObserver: MonitoringObserver = useLoaderData({ strict: false });

  const { tab, monitoringObserverId } = Route.useParams();
  const [currentTab, setCurrentTab] = useState(tab);
  const navigate = useNavigate();

  function handleTabChange(tab: string): void {
    setCurrentTab(tab);
    navigate({
      params(prev) {
        return { ...prev, tab, monitoringObserverId };
      },
    });
  }

  return (
    <Layout title={`${monitoringObserver.firstName} ${monitoringObserver.lastName}`}>
      <Tabs value={currentTab} onValueChange={handleTabChange}>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='details'>Observer details</TabsTrigger>
          <TabsTrigger value='responses'>Responses/forms</TabsTrigger>
        </TabsList>
        <TabsContent value='details'>
          <MonitoringObserverDetailsView />
        </TabsContent>
        <TabsContent value='responses'>
          <MonitoringObserverForms />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
