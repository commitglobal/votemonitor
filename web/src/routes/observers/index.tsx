import ObserversDashboard from '@/features/observers/components/Dashboard/Dashboard';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/observers/')({
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
  component: Observers,
});

function Observers() {
  return <ObserversDashboard />;
}
