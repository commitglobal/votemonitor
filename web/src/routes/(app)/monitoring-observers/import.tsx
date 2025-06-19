import { MonitoringObserversImport } from '@/features/monitoring-observers/components/MonitoringObserversImport/MonitoringObserversImport';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/monitoring-observers/import')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: MonitoringObserversImport
});
