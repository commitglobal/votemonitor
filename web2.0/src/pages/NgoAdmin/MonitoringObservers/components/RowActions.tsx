import { Link } from '@tanstack/react-router'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { Route } from '@/routes/(app)/elections/$electionRoundId/observers'
import { ElectionRoundStatus } from '@/types/election'
import {
  MonitoringObserverStatus,
  type MonitoringObserverModel,
} from '@/types/monitoring-observer'
import { Ellipsis } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { useObservers } from './ObserversProvider'

type MonitoringObserverRowActionsProps = {
  observer: MonitoringObserverModel
}

export function MonitoringObserverRowActions({
  observer,
}: MonitoringObserverRowActionsProps) {
  const { electionRoundId } = Route.useParams()
  const { electionRound } = useCurrentElectionRound()
  const { setOpen, setCurrentRow } = useObservers()

  // An archived round is frozen, so nothing about its observers can change.
  const isArchived = electionRound?.status === ElectionRoundStatus.Archived

  return (
    <DropdownMenu modal={false}>
      <DropdownMenuTrigger asChild>
        <Button
          variant='ghost'
          className='data-[state=open]:bg-muted flex h-8 w-8 p-0'
        >
          <Ellipsis className='h-4 w-4' />
          <span className='sr-only'>Open menu</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align='end' className='w-[200px]'>
        <DropdownMenuItem asChild>
          <Link
            to='/elections/$electionRoundId/observers/$observerId'
            params={{ electionRoundId, observerId: observer.id }}
          >
            View
          </Link>
        </DropdownMenuItem>

        {/* Editing an observer needs a page that does not exist yet in this app,
            so the entry keeps its place in the menu but stays inert. */}
        <DropdownMenuItem disabled>Edit</DropdownMenuItem>

        {/* Only an observer who has not accepted yet has an invitation worth
            sending again. */}
        <DropdownMenuItem
          disabled={
            isArchived || observer.status !== MonitoringObserverStatus.Pending
          }
          onClick={() => {
            setCurrentRow(observer)
            setOpen('resendInvite')
          }}
        >
          Resend invitation email
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
