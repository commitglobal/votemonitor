import type { FunctionComponent } from '../common/types';
import PollingStationsDashboard from '@/features/polling-stations/components/Dashboard/Dashboard';
import Layout from '@/components/layout/Layout';
import Panel from '@/components/layout/Panel/Panel';


const Home = (): FunctionComponent => {
  return (
    <Layout title={'Dashboard'}>
      <Panel>
        <PollingStationsDashboard />
      </Panel>
    </Layout>
  );
};

export default Home;
