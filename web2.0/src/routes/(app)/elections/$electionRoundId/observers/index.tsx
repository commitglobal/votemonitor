import { createFileRoute } from '@tanstack/react-router'
import Page from '@/pages/NgoAdmin/MonitoringObservers/Page'
import { monitoringObserversSearchSchema } from '@/types/monitoring-observer'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/observers/'
)({
  validateSearch: monitoringObserversSearchSchema,
  component: Page,
})
