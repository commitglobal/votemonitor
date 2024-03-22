import type { FunctionComponent } from '../common/types';
import { createFileRoute } from '@tanstack/react-router';
import PollingStationsDashboard from '@/features/polling-stations/components/Dashboard/Dashboard';
import Layout from '@/components/layout/Layout';
import Panel from '@/components/layout/Panel/Panel';

const Index = (): FunctionComponent => {
  return (
    <Layout title={'Dashboard'}>
      <Panel>{/* <PollingStationsDashboard /> */}</Panel>
    </Layout>
  );
};

export const Route = createFileRoute('/')({
  component: Index,
});
