import { AuthContext } from '@/context/auth.context';
import NgoAdminDashboard from '@/features/ngo-admin-dashboard/components/Dashboard/Dashboard';
import PlatformAdminDashboard from '@/features/platform-admin-dashboard/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';
import { useContext } from 'react';

import type { FunctionComponent } from '../common/types';
const Index = (): FunctionComponent => {
  const { userRole } = useContext(AuthContext);

  return userRole === 'PlatformAdmin' ? <PlatformAdminDashboard /> : <NgoAdminDashboard />
};

export const Route = createFileRoute('/')({
  beforeLoad: ({ context }) => {
    redirectIfNotAuth(context.authContext.isAuthenticated);
  },
  component: Index,
});
