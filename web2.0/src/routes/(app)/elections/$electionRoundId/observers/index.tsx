import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import Page from '@/pages/NgoAdmin/MonitoringObservers/Page'
import { listMonitoringObserversQueryOptions } from '@/queries/monitoring-observers'
import { monitoringObserversSearchSchema } from '@/types/monitoring-observer'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/observers/'
)({
  validateSearch: monitoringObserversSearchSchema,
  search: {
    // Keep the url readable while nothing is filtered.
    middlewares: [
      stripSearchParams({
        searchText: undefined,
        status: undefined,
        tags: undefined,
      }),
    ],
  },
  loaderDeps: ({ search }) => ({ ...search }),
  loader: async ({ context, deps, params: { electionRoundId } }) => {
    await context.queryClient.ensureQueryData(
      listMonitoringObserversQueryOptions(electionRoundId, deps)
    )
  },
  component: Page,
})
