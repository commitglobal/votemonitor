import Header from '@/components/layout/Header/Header'
import { TanStackReactQueryDevelopmentTools } from '@/components/utils/development-tools/TanStackReactQueryDevelopmentTools';
import { TanStackRouterDevelopmentTools } from '@/components/utils/development-tools/TanStackRouterDevelopmentTools';
import { RouterContext } from '@/routerContext';
import { createRootRouteWithContext, Outlet } from '@tanstack/react-router'
import { Suspense } from 'react';


export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootComponent,
});

function RootComponent() {
  return (
    <div className='flex flex-col min-h-screen pb-20 gap-y-10'>
      <Header />
      <Outlet />
      <Suspense>
        <TanStackReactQueryDevelopmentTools buttonPosition="bottom-left" />
        <TanStackRouterDevelopmentTools position="bottom-right" initialIsOpen={false} />
      </Suspense>
    </div>
  );
}