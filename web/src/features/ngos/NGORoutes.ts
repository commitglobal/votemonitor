/* eslint-disable unicorn/prefer-top-level-await */
import { rootRoute } from '@/routes/RootRoute';
import { Route } from '@tanstack/router';
import React from 'react';
import { ngoRouteSearchSchema } from './models/NGO';

const NGODashboard = React.lazy(() => import('./components/Dashboard/Dashboard'));

export const NGODashboardRoute = new Route({
  getParentRoute: (): typeof rootRoute => rootRoute,
  path: '/ngos',
  component: NGODashboard,
  validateSearch: ngoRouteSearchSchema,
});

export default [NGODashboardRoute];
