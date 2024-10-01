import MonitoringObserversDashboard from '@/features/monitoring-observers/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, redirect } from '@tanstack/react-router';

export const Route = createFileRoute('/monitoring-observers/$tab')({
  beforeLoad: ({ params: { tab } }) => {
    redirectIfNotAuth();

    const coercedTab = coerceTabSlug(tab);
    if (tab !== coercedTab) {
      throw redirect({ to: `/monitoring-observers/$tab`, params: { tab: coercedTab } });
    }
  },
  component: MonitoringObservers,
});

const coerceTabSlug = (slug: string) => {
  if (slug?.toLowerCase()?.trim() === 'list') return 'list';
  if (slug?.toLowerCase()?.trim() === 'push-messages') return 'push-messages';

  return 'list';
};

function MonitoringObservers() {
  return (
    <div className='p-2'>
      <MonitoringObserversDashboard />
    </div>
  );
}
