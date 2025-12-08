import { useEffect } from 'react'
import { createFileRoute, Outlet, redirect } from '@tanstack/react-router'
import {
  CurrentElectionRoundProvider,
  useCurrentElectionRound,
} from '@/contexts/election-round.context'
import { useSuspenseElectionRoundDetails } from '@/queries/elections'
import { ElectionSiteHeader } from '@/components/ElectionSiteHeader'

export const Route = createFileRoute('/(app)/elections/$electionRoundId')({
  component: RouteComponentWrapper,
})

function RouteComponentWrapper() {
  return (
    <CurrentElectionRoundProvider>
      <RouteComponent />
    </CurrentElectionRoundProvider>
  )
}

function RouteComponent() {
  const { electionRoundId } = Route.useParams()
  const { data: electionRound, isLoading } =
    useSuspenseElectionRoundDetails(electionRoundId)
  const { setElectionRound } = useCurrentElectionRound()

  useEffect(() => {
    setElectionRound(electionRound)
  }, [electionRound, setElectionRound])

  console.log(electionRound)

  if (isLoading) {
    return <div>Loading...</div>
  }

  if (!electionRound) {
    throw redirect({ to: '/elections' })
  }
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
