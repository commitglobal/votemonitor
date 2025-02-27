import Header from '@/components/layout/Header/Header';
import { Toaster } from '@/components/ui/toaster';
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
    <Suspense>
      <Toaster />
      <div className='flex flex-col min-h-screen pb-20'>
        {isAuthenticated && <Header />}
        <Outlet />
      </div>
    </Suspense>
  );
}
