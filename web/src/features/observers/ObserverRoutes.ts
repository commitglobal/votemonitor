/* eslint-disable unicorn/prefer-top-level-await */
import { rootRoute } from '@/routes/RootRoute';
import { Route } from '@tanstack/router';
import React from 'react';
import { observerSearchSchema } from './models/Observer';

const ObserversDashboard = React.lazy(() => import('./components/Dashboard/Dashboard'));

export const ObserverDashboardRoute = new Route({
  getParentRoute: (): typeof rootRoute => rootRoute,
  path: '/observers',
  component: ObserversDashboard,
  validateSearch: observerSearchSchema,
});

export default [ObserverDashboardRoute];
