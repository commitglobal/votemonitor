import { BreadcrumbsWithAliases } from '@/components/layout/Breadcrumbs/BreadcrumbsWithAliases';
import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Route, observerDetailsQueryOptions } from '@/routes/(app)/observers/$observerId';
import { useSuspenseQuery } from '@tanstack/react-query';
import { ObserverPersonalDetails } from './ObserverPersonalDetails';
import { ObserversMonitoredElections } from './ObserversMonitoredElections';

export default function ObserverProfile() {
  const { observerId } = Route.useParams();
  const observerQuery = useSuspenseQuery(observerDetailsQueryOptions(observerId));
  const observer = observerQuery.data;

  const displayName = observer.firstName + ' ' + observer.lastName;

  return (
    <Layout
      title={displayName}
      breadcrumbs={<BreadcrumbsWithAliases customAliases={[{ param: observerId, alias: displayName }]} />}>
      <>
        <Tabs defaultValue='details'>
          <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
            <TabsTrigger value='details'>Observer details</TabsTrigger>
            <TabsTrigger value='elections'>Monitored elections</TabsTrigger>
          </TabsList>
          <TabsContent value='details'>
            <ObserverPersonalDetails observer={observer} />
          </TabsContent>
          <TabsContent value='elections'>
            <ObserversMonitoredElections observer={observer} />
          </TabsContent>
        </Tabs>
      </>
    </Layout>
  );
}
