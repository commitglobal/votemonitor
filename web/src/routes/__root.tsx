import { Toaster } from '@/components/ui/toaster';
import { RouterContext } from '@/routerContext';
import { createRootRouteWithContext, Outlet } from '@tanstack/react-router';
import { Suspense } from 'react';

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootComponent,
});

function RootComponent() {
  return (
    <Suspense>
      <Toaster />
      <Outlet />
    </Suspense>
  );
}
