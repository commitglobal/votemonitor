import type { FunctionComponent } from '../common/types';
import PollingStationsDashboard from '@/features/polling-stations/components/Dashboard/Dashboard';

const Home = (): FunctionComponent => {
  return (
    <>
      <header>
        <div className='mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8'>
          <h1 className='text-3xl font-bold tracking-tight text-gray-900'>Dashboard</h1>
        </div>
      </header>
      <main>
        <div className='mx-auto max-w-7xl py-6 sm:px-6 lg:px-8 '>
          <PollingStationsDashboard />
        </div>
      </main>
    </>
  );
};

export default Home;
