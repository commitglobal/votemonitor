import type { FunctionComponent } from '../common/types';
import { createFileRoute } from '@tanstack/react-router';
import Layout from '@/components/layout/Layout';
import PollingStationsDashboard from '@/features/polling-stations/components/Dashboard/Dashboard';

const Index = (): FunctionComponent => {
  return (
    <Layout title={'Dashboard'}>
      <PollingStationsDashboard />
    </Layout>
  );
};

export const Route = createFileRoute('/')({
  component: Index,
});
