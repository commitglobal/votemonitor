import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, Navigate } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/monitoring-observers/')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Component,
});

function Component() {
  return <Navigate to={`/monitoring-observers/$tab`} params={{ tab: 'list' }} replace={true} />;
}
