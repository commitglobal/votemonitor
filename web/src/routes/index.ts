import { Router } from '@tanstack/router';
import { IndexRoute } from './IndexRoute';
import { rootRoute } from './RootRoute';
import ObserverRoutes from '@/features/observers/ObserverRoutes';
import NGORoutes from '@/features/ngos/NGORoutes';

const routeTree = rootRoute.addChildren([IndexRoute, ...ObserverRoutes, ...NGORoutes]);

const router = new Router({ routeTree });

declare module '@tanstack/router' {
  interface Register {
    // This infers the type of our router and registers it across your entire project
    router: typeof router;
  }
}

export { router };
