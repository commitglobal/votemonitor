import { ElectionRoundStatus } from '@/common/types'
import ElectionRoundsDashboard from '@/features/election-rounds/components/Dashboard/Dashboard'
import { redirectIfNotAuth } from '@/lib/utils'
import { createFileRoute } from '@tanstack/react-router'
import { z } from 'zod'

const electionRoundsDashboardRouteSearchSchema = z.object({
  searchText: z.string().catch(''),
  electionRoundStatus: z.nativeEnum(ElectionRoundStatus).optional(),
  countryId: z.string().optional(),
})

export const Route = createFileRoute('/(app)/election-rounds/')({
  component: ElectionRoundsDashboard,
  validateSearch: electionRoundsDashboardRouteSearchSchema,
  beforeLoad: () => {
    redirectIfNotAuth()
  },
})
