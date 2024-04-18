import type { FunctionComponent } from '../common/types';
import { createFileRoute } from '@tanstack/react-router';
import Layout from '@/components/layout/Layout';
import PollingStationsDashboard from '@/features/polling-stations/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';

const Index = (): FunctionComponent => {
  return (
    <Layout title={'Dashboard'}>
      <PollingStationsDashboard />
    </Layout>
  );
};

export const Route = createFileRoute('/')({
  beforeLoad: ({ context }) => {
    redirectIfNotAuth(context.authContext.isAuthenticated);
  },
  component: Index,
});
