import { createFileRoute, Outlet, redirect } from '@tanstack/react-router'
import {
  CurrentElectionRoundProvider,
  useCurrentElectionRound,
} from '@/contexts/election-round.context'
import { queryClient } from '@/main'
import {
  electionRoundDetailsQueryOptions,
  useSuspenseElectionRoundDetails,
} from '@/queries/elections'
import { ElectionSiteHeader } from '@/components/ElectionSiteHeader'

export const Route = createFileRoute('/(app)/elections/$electionRoundId')({
  component: RouteComponentWrapper,
  loader: ({ params: { electionRoundId } }) =>
    queryClient.ensureQueryData(
      electionRoundDetailsQueryOptions(electionRoundId)
    ),
})

function RouteComponentWrapper() {
  const { electionRoundId } = Route.useParams()
  const { data: electionRound, isLoading } =
    useSuspenseElectionRoundDetails(electionRoundId)

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!electionRound) {
    throw redirect({ to: '/elections' })
  }

  return (
    <CurrentElectionRoundProvider>
      <RouteComponent />
    </CurrentElectionRoundProvider>
  )
}

function RouteComponent() {
  const electionRound = Route.useLoaderData()
  const { setElectionRound } = useCurrentElectionRound()
  setElectionRound(electionRound)
  return (
    <>
      <ElectionSiteHeader />
      <div className='container-wrapper'>
        <div className='container py-6'>
          <section>
            <Outlet />
          </section>
        </div>
      </div>
    </>
  )
}
