import { LocationsImport } from '@/features/locations/LocationsImport/LocationsImport'
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute(
  '/(app)/election-rounds/$electionRoundId/locations/import',
)({
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
  component: LocationsImport,
})
