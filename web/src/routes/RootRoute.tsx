import { Outlet, RootRoute } from '@tanstack/router';
import type { FunctionComponent } from '../common/types';
import Header from '../components/layout/Header/Header';

export const rootRoute = new RootRoute({
  component: (): FunctionComponent => (
    <div className='flex flex-col min-h-screen pb-20 gap-y-10'>
      <Header />
      <Outlet />
    </div>
  ),
});
