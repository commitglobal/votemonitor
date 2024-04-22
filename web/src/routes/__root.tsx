import Header from '@/components/layout/Header/Header';
import { Toaster } from '@/components/ui/toaster';
import { TanStackReactQueryDevelopmentTools } from '@/components/utils/development-tools/TanStackReactQueryDevelopmentTools';
import { TanStackRouterDevelopmentTools } from '@/components/utils/development-tools/TanStackRouterDevelopmentTools';
import { AuthContext } from '@/context/auth.context';
import { RouterContext } from '@/routerContext';
import { createRootRouteWithContext, Outlet } from '@tanstack/react-router';
import { Suspense, useContext } from 'react';

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootComponent,
});

function RootComponent() {
  const { isAuthenticated } = useContext(AuthContext);
  return (
    <>
      <Toaster />
      <div className='flex flex-col min-h-screen pb-20'>
        {isAuthenticated && <Header />}
        <Outlet />
      </div>
    </>
  );
}
