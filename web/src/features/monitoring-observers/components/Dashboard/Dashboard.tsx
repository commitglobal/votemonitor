import Layout from '@/components/layout/Layout';

import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Route } from '@/routes/monitoring-observers/$tab';
import { useNavigate } from '@tanstack/react-router';
import { ReactElement, useState } from 'react';

import MonitoringObserversList from '../MonitoringObserversList/MonitoringObserversList';
import PushMessages from '../PushMessages/PushMessages';

export default function MonitoringObserversDashboard(): ReactElement {
  const { tab } = Route.useParams();
  const [currentTab, setCurrentTab] = useState(tab);
  const navigate = useNavigate();

  function handleTabChange(tab: string): void {
    setCurrentTab(tab);
    navigate({
      to: '.',
      replace: true,
      params(prev) {
        return { ...prev, tab };
      },
    });
  }

  return (
    <Layout
      title='Observers'
      subtitle='View all monitoring observers you imported as an NGO admin and invite them to current election observation event.'
      enableBreadcrumbs={false}>
      <Tabs defaultValue='list' value={currentTab} onValueChange={handleTabChange}>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='list'>Monitoring observers</TabsTrigger>
          <TabsTrigger value='push-messages'>Push messages</TabsTrigger>
        </TabsList>
        <TabsContent value='list'>
          <MonitoringObserversList />
        </TabsContent>
        <TabsContent value='push-messages'>
          <PushMessages />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
