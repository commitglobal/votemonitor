import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, Navigate } from '@tanstack/react-router';

import type { FunctionComponent } from '@/common/types';
export const Route = createFileRoute('/monitoring-observers/$monitoringObserverId/view')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Component,
});

function Component() {
  const { monitoringObserverId } = Route.useParams();

  return <Navigate to={`/monitoring-observers/$monitoringObserverId/view/$tab`} params={{ monitoringObserverId, tab: 'details' }} />;
}
