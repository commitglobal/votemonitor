import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useLoaderData } from '@tanstack/react-router';

import MonitoringObserverDetailsView from '../MonitoringObserverDetailsView/MonitoringObserverDetailsView';
import { MonitoringObserverForms } from '../MonitoringObserverForms/MonitoringObserverForms';

import type { FunctionComponent } from '@/common/types';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';
import { monitoringObserverDetailsQueryOptions } from '@/common/queryOptions';

export default function MonitoringObserverDetails(): FunctionComponent {
  const { monitoringObserverId } = Route.useParams();
  const monitoringObserverQuery = useSuspenseQuery(monitoringObserverDetailsQueryOptions(monitoringObserverId));
  const monitoringObserver = monitoringObserverQuery.data;

  return (
    <Layout title={`${monitoringObserver.firstName} ${monitoringObserver.lastName}`}>
      <Tabs defaultValue='details'>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='details'>Observer details</TabsTrigger>
          <TabsTrigger value='responses'>Form responses</TabsTrigger>
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