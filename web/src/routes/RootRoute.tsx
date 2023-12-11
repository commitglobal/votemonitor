import { Outlet, RootRoute } from '@tanstack/router';
import type { FunctionComponent } from '../common/types';
import Header from '../components/layout/Header/Header';

export const rootRoute = new RootRoute({
  component: (): FunctionComponent => (
    <div className='min-h-full'>
      <Header />
      <Outlet />
    </div>
  ),
});
