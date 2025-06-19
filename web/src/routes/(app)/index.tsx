import { AuthContext, useAuth } from '@/context/auth-context';
import NgoAdminDashboard from '@/features/ngo-admin-dashboard/components/Dashboard/Dashboard';
import PlatformAdminDashboard from '@/features/platform-admin-dashboard/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';
import { useContext } from 'react';

import { DataSources, type FunctionComponent } from '../../common/types';
import { z } from 'zod';
const StatisticsDetails = (): FunctionComponent => {
  const { isPlatformAdmin } = useAuth();

  return isPlatformAdmin ? <PlatformAdminDashboard /> : <NgoAdminDashboard />;
};

export const ZDataSourceSearchSchema = z.object({
  dataSource: z.nativeEnum(DataSources).catch(DataSources.Ngo).optional(),
});

export const Route = createFileRoute('/(app)/')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  validateSearch: ZDataSourceSearchSchema,
  component: StatisticsDetails,
});
