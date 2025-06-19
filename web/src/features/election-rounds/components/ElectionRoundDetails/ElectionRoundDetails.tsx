import ElectionEventDescription from '@/components/ElectionEventDescription/ElectionEventDescription';
import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import LocationsDashboard from '@/components/LocationsDashboard/LocationsDashboard';
import { Route } from '@/routes/(app)/election-rounds/$electionRoundId/$tab';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { electionRoundDetailsQueryOptions } from '../../queries';
import PollingStationsDashboard from '@/components/PollingStationsDashboard/PollingStationsDashboard';
import { useNavigate } from '@tanstack/react-router';
import PsiFormDashboard from '../PsiFormDashboard/PsiFormDashboard';
import MonitoringNgosDashboard from '../MonitoringNgosDashboard/MonitoringNgosDashboard';
import CitizenReportingDashboard from '../CitizenReportingDashboard/CitizenReportingDashboard';
import AssignedFormTemplatesDashboard from '../AssignedFormTemplatesDashboard/AssignedFormTemplatesDashboard';
import { ElectionRoundDetailsTab } from './tabs';
function ElectionRoundDetails() {
  const navigate = useNavigate();
  const { electionRoundId, tab } = Route.useParams();
  const { setCurrentElectionRoundId } = useCurrentElectionRoundStore((s) => s);

  const { data: electionRound } = useSuspenseQuery(electionRoundDetailsQueryOptions(electionRoundId));
  const { t } = useTranslation();

  useEffect(() => {
    setCurrentElectionRoundId(electionRoundId);
  }, [electionRoundId, setCurrentElectionRoundId]);

  return (
    <Layout title={electionRound.title} subtitle={electionRound.englishTitle} enableBreadcrumbs={false}>
      <Tabs
        defaultValue={tab ?? ElectionRoundDetailsTab.EventDetails}
        onValueChange={(tab) => {
          navigate({
            to: '/election-rounds/$electionRoundId/$tab',
            params: { electionRoundId, tab },
          });
        }}>
        <TabsList className='grid grid-cols-7 bg-gray-200'>
          <TabsTrigger className='whitespace-normal text-wrap' value={ElectionRoundDetailsTab.EventDetails}>
            {t('electionEvent.eventDetails.tabTitle')}
          </TabsTrigger>
          <TabsTrigger className='whitespace-normal text-wrap' value={ElectionRoundDetailsTab.PsiForm}>
            {t('electionEvent.psiForm.tabTitle')}
          </TabsTrigger>
          <TabsTrigger className='whitespace-normal text-wrap' value={ElectionRoundDetailsTab.PollingStations}>
            {t('electionEvent.pollingStations.tabTitle')}
          </TabsTrigger>
          <TabsTrigger className='whitespace-normal text-wrap' value={ElectionRoundDetailsTab.MonitoringNgos}>
            {t('electionEvent.monitoringNgos.tabTitle')}
          </TabsTrigger>
          <TabsTrigger className='whitespace-normal text-wrap' value={ElectionRoundDetailsTab.CitizenReporting}>
            {t('electionEvent.citizenReporting.tabTitle')}
          </TabsTrigger>
          <TabsTrigger className='whitespace-normal text-wrap' value={ElectionRoundDetailsTab.Locations}>
            {t('electionEvent.locations.tabTitle')}
          </TabsTrigger>
          <TabsTrigger className='whitespace-normal text-wrap' value={ElectionRoundDetailsTab.FormTemplates}>
            {t('electionEvent.formTemplates.tabTitle')}
          </TabsTrigger>
        </TabsList>

        <TabsContent value={ElectionRoundDetailsTab.EventDetails}>
          <ElectionEventDescription />
        </TabsContent>

        <TabsContent value={ElectionRoundDetailsTab.PsiForm}>
          <PsiFormDashboard electionRoundId={electionRoundId} />
        </TabsContent>

        <TabsContent value={ElectionRoundDetailsTab.PollingStations}>
          <PollingStationsDashboard />
        </TabsContent>

        <TabsContent value={ElectionRoundDetailsTab.MonitoringNgos}>
          <MonitoringNgosDashboard electionRoundId={electionRoundId} />
        </TabsContent>

        <TabsContent value={ElectionRoundDetailsTab.CitizenReporting}>
          <CitizenReportingDashboard electionRoundId={electionRoundId} />
        </TabsContent>

        <TabsContent value={ElectionRoundDetailsTab.Locations}>
          <LocationsDashboard />
        </TabsContent>

        <TabsContent value={ElectionRoundDetailsTab.FormTemplates}>
          <AssignedFormTemplatesDashboard electionRoundId={electionRoundId} />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}

export default ElectionRoundDetails;
