import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { getRouteApi, Outlet } from '@tanstack/react-router';
import { ChangeEvent, ReactElement, useState } from 'react';
import ElectionEventDetails from '../ElectionEventDetails/ElectionEventDetails';
import PollingStationsDashboard from '@/features/polling-stations/components/Dashboard/Dashboard';
import { useElectionRound } from '../../hooks/election-event-hooks';
import ObserversGuides from '../ObserversGuides/ObserversGuides';

const routeApi = getRouteApi('/election-event/$tab');

export default function ElectionEventDashboard(): ReactElement {
  const { tab } = routeApi.useParams();
  const navigate = routeApi.useNavigate();

  const [currentTab, setCurrentTab] = useState(tab);

  function handleTabChange(tab: string): void {
    setCurrentTab(tab);
    navigate({
      params(prev) {
        return { ...prev, tab };
      },
    });
  }
  const { data: electionEvent } = useElectionRound();

  return (
    <Layout title={electionEvent?.title ?? ''} breadcrumbs={<></>} backButton={<></>}>
      <Tabs defaultValue='event-details' value={currentTab} onValueChange={handleTabChange}>
        <TabsList className='grid grid-cols-4 bg-gray-200 w-[800px] mb-4'>
          <TabsTrigger value='event-details'>Event Details</TabsTrigger>
          <TabsTrigger value='polling-stations'>Polling Stations</TabsTrigger>
          <TabsTrigger value='observer-guides'>Observer Guides</TabsTrigger>
          <TabsTrigger value='observer-forms'>Observer Forms</TabsTrigger>
        </TabsList>

        <TabsContent value='event-details'><ElectionEventDetails /></TabsContent>
        <TabsContent value='polling-stations'><PollingStationsDashboard /></TabsContent>
        <TabsContent value='observer-guides'><ObserversGuides /></TabsContent>
        <TabsContent value='observer-forms'><h1>tbd</h1></TabsContent>
      </Tabs>
    </Layout>
  );
}
