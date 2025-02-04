import ElectionRoundEdit from '@/features/election-rounds/components/ElectionRoundEdit/ElectionRoundEdit'
import { electionRoundDetailsQueryOptions } from '@/features/election-rounds/queries'
import { redirectIfNotAuth } from '@/lib/utils'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/election-rounds/$electionRoundId/edit')({
  component: ElectionRoundEdit,
  loader: ({ context: { queryClient }, params: { electionRoundId } }) =>
    queryClient.ensureQueryData(
      electionRoundDetailsQueryOptions(electionRoundId),
    ),
  beforeLoad: () => {
    redirectIfNotAuth()
  },
})
