import { createFileRoute } from '@tanstack/react-router'
import Page from '@/pages/NgoAdmin/Dashboard/Page'
import { electionRoundStatisticsQueryOptions } from '@/queries/statistics'
import { dashboardSearchSchema } from '@/types/statistics'

export const Route = createFileRoute('/(app)/elections/$electionRoundId/')({
  validateSearch: dashboardSearchSchema,
  loaderDeps: ({ search: { dataSource } }) => ({ dataSource }),
  loader: async ({ context, deps, params: { electionRoundId } }) => {
    await context.queryClient.ensureQueryData(
      electionRoundStatisticsQueryOptions(electionRoundId, deps.dataSource)
    )
  },
  component: Page,
})
