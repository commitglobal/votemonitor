import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import FormsDashboard from '@/features/forms/components/Dashboard/Dashboard';
import LocationsDashboard from '@/features/locations/components/Dashboard/Dashboard';
import PollingStationsDashboard from '@/features/polling-stations/components/Dashboard/Dashboard';
import { cn } from '@/lib/utils';
import { getRouteApi } from '@tanstack/react-router';
import { ReactElement, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useElectionRoundDetails } from '../../hooks/election-event-hooks';
import GuidesDashboard from '../Guides/GuidesDashboard';
import ElectionEventDetails from '../ElectionEventDetails/ElectionEventDetails';
import { GuidePageType } from '../../models/guide';

const routeApi = getRouteApi('/election-event/$tab');

export default function ElectionEventDashboard(): ReactElement {
  const { t } = useTranslation();
  const { tab } = routeApi.useParams();
  const [currentTab, setCurrentTab] = useState(tab);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const isMonitoringNgoForCitizenReporting = useCurrentElectionRoundStore((s) => s.isMonitoringNgoForCitizenReporting);

  const navigate = routeApi.useNavigate();

  function handleTabChange(tab: string): void {
    setCurrentTab(tab);
    navigate({
      params(prev) {
        return { ...prev, tab };
      },
    });
  }
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);


  return (
    <Layout title={electionEvent?.title ?? ''} breadcrumbs={<></>} backButton={<></>}>
      <Tabs defaultValue='event-details' value={currentTab} onValueChange={handleTabChange}>
        <TabsList
          className={cn('grid grid-cols-4 bg-gray-200 w-[800px]', {
            'grid-cols-6 w-[1200px]': isMonitoringNgoForCitizenReporting,
          })}>
          <TabsTrigger value='event-details'>{t('electionEvent.eventDetails.tabTitle')}</TabsTrigger>
          <TabsTrigger value='polling-stations'>{t('electionEvent.pollingStations.tabTitle')}</TabsTrigger>
          <TabsTrigger value='locations'>{t('electionEvent.locations.tabTitle')}</TabsTrigger>
          <TabsTrigger value='observer-guides'>{t('electionEvent.guides.observerGuidesTabTitle')}</TabsTrigger>
          <TabsTrigger value='citizen-guides'>{t('electionEvent.guides.citizenGuidesTabTitle')}</TabsTrigger>
          <TabsTrigger value='observer-forms'>{t('electionEvent.observerForms.tabTitle')}</TabsTrigger>
        </TabsList>

        <TabsContent value='event-details'>
          <ElectionEventDetails />
        </TabsContent>

        <TabsContent value='polling-stations'>
          <PollingStationsDashboard />
        </TabsContent>

        {isMonitoringNgoForCitizenReporting && (
          <TabsContent value='locations'>
            <LocationsDashboard />
          </TabsContent>
        )}

        <TabsContent value='observer-guides'>
          <GuidesDashboard guidePageType={GuidePageType.Observer} />
        </TabsContent>

        {isMonitoringNgoForCitizenReporting && (
          <TabsContent value='citizen-guides'>
            <GuidesDashboard guidePageType={GuidePageType.Citizen} />
          </TabsContent>
        )}

        <TabsContent value='observer-forms'>
          <FormsDashboard />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
