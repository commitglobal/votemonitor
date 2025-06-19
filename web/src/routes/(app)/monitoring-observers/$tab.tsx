import MonitoringObserversDashboard from '@/features/monitoring-observers/components/Dashboard/Dashboard';
import { MonitoringObserverStatus } from '@/features/monitoring-observers/models/monitoring-observer';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, redirect } from '@tanstack/react-router';
import { z } from 'zod';

export const MonitoringObserversSearchParamsSchema = z.object({
  searchText: z.coerce.string().optional(),
  tags: z.array(z.string()).optional(),
  monitoringObserverStatus: z.nativeEnum(MonitoringObserverStatus).optional(),
});

export type MonitoringObserversSearchParams = z.infer<typeof MonitoringObserversSearchParamsSchema>;

export const Route = createFileRoute('/(app)/monitoring-observers/$tab')({
  beforeLoad: ({ params: { tab } }) => {
    redirectIfNotAuth();

    const coercedTab = coerceTabSlug(tab);
    if (tab !== coercedTab) {
      throw redirect({ to: `/monitoring-observers/$tab`, params: { tab: coercedTab } });
    }
  },
  validateSearch: MonitoringObserversSearchParamsSchema,
  component: MonitoringObservers,
});

const coerceTabSlug = (slug: string) => {
  if (slug?.toLowerCase()?.trim() === 'list') return 'list';
  if (slug?.toLowerCase()?.trim() === 'push-messages') return 'push-messages';

  return 'list';
};

function MonitoringObservers() {
  return <MonitoringObserversDashboard />;
}
