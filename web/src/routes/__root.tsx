import { Toaster } from '@/components/ui/sonner';
import { RouterContext } from '@/routerContext';
import { createRootRouteWithContext, Outlet } from '@tanstack/react-router';
import { Suspense } from 'react';

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootComponent,
});

function RootComponent() {
  return (
    <Suspense>
      <Toaster richColors toastOptions={{}} theme='light' closeButton duration={2000} />
      <Outlet />
    </Suspense>
  );
}
