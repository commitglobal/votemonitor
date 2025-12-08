import { Link } from '@tanstack/react-router'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { cn } from '@/lib/utils'

export interface PlatformAdminNavProps {
  electionRoundId: string
}
export default function PlatformAdminNav() {
  const { electionRound } = useCurrentElectionRound()

  return (
    <div className='mr-4 hidden md:flex'>
      <nav className='flex items-center gap-4 text-sm xl:gap-6'>
        <Link
          to='/elections/$electionRoundId'
          params={{ electionRoundId: electionRound?.id! }}
          className={cn('hover:text-foreground/80 transition-colors')}
        >
          Dashboard
        </Link>
        <Link
          to='/elections/$electionRoundId/observers'
          params={{ electionRoundId: electionRound?.id! }}
          className={cn(
            'hover:text-foreground/80 text-foreground/80 transition-colors'
          )}
          activeProps={{
            className: 'text-foreground',
          }}
        >
          Observers
        </Link>
      </nav>
    </div>
  )
}
