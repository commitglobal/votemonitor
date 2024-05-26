import ElectionEventDashboard from '@/features/election-event/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, redirect } from '@tanstack/react-router';

const coerceTabSlug = (slug: string) => {
  if (slug?.toLowerCase()?.trim() === 'event-details') return 'event-details';
  if (slug?.toLowerCase()?.trim() === 'polling-stations') return 'polling-stations';
  if (slug?.toLowerCase()?.trim() === 'observer-guides') return 'observer-guides';
  if (slug?.toLowerCase()?.trim() === 'observer-forms') return 'observer-forms';

  return 'event-details'
};

export const Route = createFileRoute('/election-event/$tab')({
  component: ElectionEventDashboard,
  beforeLoad: ({ params: { tab } }) => {
    redirectIfNotAuth();

    const coercedTab = coerceTabSlug(tab);
    if (tab !== coercedTab) {
      throw redirect({ to: `/election-event/$tab`, params: { tab: coercedTab } })
    }
  },
});
