import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import FormsDashboard from '@/features/forms/components/Dashboard/Dashboard';
import LocationsDashboard from '@/components/LocationsDashboard/LocationsDashboard';
import { cn } from '@/lib/utils';
import { getRouteApi, useNavigate } from '@tanstack/react-router';
import { ReactElement, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useElectionRoundDetails } from '../../hooks/election-event-hooks';
import GuidesDashboard from '../Guides/GuidesDashboard';
import ElectionEventDescription from '../../../../components/ElectionEventDescription/ElectionEventDescription';
import { GuidePageType } from '../../models/guide';
import CitizenNotificationsDashboard from '@/features/CitizenNotifications/CitizenNotificationsDashboard/CitizenNotificationsDashboard';
import { Route } from '@/routes/(app)/election-event/$tab';
import PollingStationsDashboard from '@/components/PollingStationsDashboard/PollingStationsDashboard';

export default function ElectionEventDashboard(): ReactElement {
  const { t } = useTranslation();
  const { tab } = Route.useParams();
  const [currentTab, setCurrentTab] = useState(tab);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const navigate = useNavigate();

  function handleTabChange(tab: string): void {
    setCurrentTab(tab);
    navigate({
      // @ts-ignore
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
          className={cn('grid grid-cols-4 bg-gray-200', {
            'grid-cols-7': electionEvent?.isMonitoringNgoForCitizenReporting,
          })}>
          <TabsTrigger value='event-details'>{t('electionEvent.eventDetails.tabTitle')}</TabsTrigger>
          <TabsTrigger value='polling-stations'>{t('electionEvent.pollingStations.tabTitle')}</TabsTrigger>
          {electionEvent?.isMonitoringNgoForCitizenReporting && (
            <TabsTrigger value='locations'>{t('electionEvent.locations.tabTitle')}</TabsTrigger>
          )}
          <TabsTrigger value='observer-guides'>{t('electionEvent.guides.observerGuidesTabTitle')}</TabsTrigger>
          {electionEvent?.isMonitoringNgoForCitizenReporting && (
            <>
              <TabsTrigger value='citizen-guides'>{t('electionEvent.guides.citizenGuidesTabTitle')}</TabsTrigger>
              <TabsTrigger value='citizen-notifications'>
                {t('electionEvent.guides.citizenNotificationsTabTitle')}
              </TabsTrigger>
            </>
          )}
          <TabsTrigger value='observer-forms'>{t('electionEvent.observerForms.tabTitle')}</TabsTrigger>
        </TabsList>

        <TabsContent value='event-details'>
          <ElectionEventDescription />
        </TabsContent>

        <TabsContent value='polling-stations'>
          <PollingStationsDashboard />
        </TabsContent>

        {electionEvent?.isMonitoringNgoForCitizenReporting && (
          <TabsContent value='locations'>
            <LocationsDashboard />
          </TabsContent>
        )}

        <TabsContent value='observer-guides'>
          <GuidesDashboard guidePageType={GuidePageType.Observer} />
        </TabsContent>

        {electionEvent?.isMonitoringNgoForCitizenReporting && (
          <>
            <TabsContent value='citizen-guides'>
              <GuidesDashboard guidePageType={GuidePageType.Citizen} />
            </TabsContent>

            <TabsContent value='citizen-notifications'>
              <CitizenNotificationsDashboard />
            </TabsContent>
          </>
        )}

        <TabsContent value='observer-forms'>
          <FormsDashboard />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
