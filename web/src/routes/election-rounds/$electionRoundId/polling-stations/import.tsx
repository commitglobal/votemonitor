import { PollingStationsImport } from '@/features/polling-stations/PollingStationsImport/PollingStationsImport';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/election-rounds/$electionRoundId/polling-stations/import')({
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
  component: PollingStationsImport,
});
