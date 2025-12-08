import { Link } from '@tanstack/react-router'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms'
import { ElectionRoundStatus } from '@/types/election'
import { PlusIcon } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { H1, P } from '@/components/ui/typography'
import FormsTable from './components/Table'

function Page() {
  const { electionRoundId } = Route.useParams()
  const { electionRound } = useCurrentElectionRound()
  const isArchived = electionRound?.status === ElectionRoundStatus.Archived

  return (
    <>
      <div className='flex items-center justify-between'>
        <div>
          <H1>Forms</H1>
          <P>Here&apos;s all forms from your election round</P>
        </div>
        {!isArchived ? (
          <Button asChild>
            <Link
              to='/elections/$electionRoundId/forms'
              params={{ electionRoundId }}
            >
              <PlusIcon className='mr-2 h-4 w-4' />
              Add form
            </Link>
          </Button>
        ) : null}
      </div>
      <FormsTable />
    </>
  )
}

export default Page
