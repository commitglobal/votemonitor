import ElectionEventDescription from '@/components/ElectionEventDescription/ElectionEventDescription';
import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import LocationsDashboard from '@/components/LocationsDashboard/LocationsDashboard';
import { Route } from '@/routes/election-rounds/$electionRoundId';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { electionRoundDetailsQueryOptions } from '../../queries';
import PollingStationsDashboard from '@/components/PollingStationsDashboard/PollingStationsDashboard';

function ElectionRoundDetails() {
  const { electionRoundId } = Route.useParams();
  const { setCurrentElectionRoundId } = useCurrentElectionRoundStore((s) => s);

  const { data: electionRound } = useSuspenseQuery(electionRoundDetailsQueryOptions(electionRoundId));
  const { t } = useTranslation();

  useEffect(() => {
    setCurrentElectionRoundId(electionRoundId);
  }, [electionRoundId, setCurrentElectionRoundId]);

  return (
    <Layout title={electionRound.title} subtitle={electionRound.englishTitle} enableBreadcrumbs={false}>
      <Tabs defaultValue='event-details'>
        <TabsList className='grid grid-cols-4 bg-gray-200 w-[800px]'>
          <TabsTrigger value='event-details'>{t('electionEvent.eventDetails.tabTitle')}</TabsTrigger>
          <TabsTrigger value='polling-stations'>{t('electionEvent.pollingStations.tabTitle')}</TabsTrigger>
          <TabsTrigger value='locations'>{t('electionEvent.locations.tabTitle')}</TabsTrigger>
          <TabsTrigger value='form-templates'>{t('electionEvent.observerForms.tabTitle')}</TabsTrigger>
        </TabsList>

        <TabsContent value='event-details'>
          <ElectionEventDescription />
        </TabsContent>

        <TabsContent value='polling-stations'>
          <PollingStationsDashboard />
        </TabsContent>

        <TabsContent value='locations'>
          <LocationsDashboard />
        </TabsContent>

        <TabsContent value='form-templates'>{/* <FormsDashboard /> */}</TabsContent>
      </Tabs>
    </Layout>
  );
}

export default ElectionRoundDetails;
