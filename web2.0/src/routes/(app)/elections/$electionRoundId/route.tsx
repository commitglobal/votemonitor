import { createFileRoute, Outlet, redirect } from '@tanstack/react-router'
import { CurrentElectionRoundProvider } from '@/contexts/election-round.context'
import { useSuspenseElectionRoundDetails } from '@/queries/elections'
import { ElectionSiteHeader } from '@/components/ElectionSiteHeader'

export const Route = createFileRoute('/(app)/elections/$electionRoundId')({
  component: RouteComponentWrapper,
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
