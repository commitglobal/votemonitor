import { Link, LinkProps } from '@tanstack/react-router'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { cn } from '@/lib/utils'

export function AdminNavLink(props: LinkProps & { title: string }) {
  const { electionRoundId } = useCurrentElectionRound()
  const { title } = props

  return (
    <Link
      params={{ electionRoundId: electionRoundId! }}
      className={cn('hover:text-foreground/80 transition-colors')}
      activeProps={{
        className: cn(
          'underline underline-offset-12 decoration-2 decoration-black dark:decoration-white text-foreground'
        ),
      }}
      {...props}
    >
      {title}
    </Link>
  )
}

export default function NgoAdminNav() {
  const { electionRoundId } = useCurrentElectionRound()

  return (
    <div className='mr-4 hidden md:flex'>
      <nav className='flex items-center gap-4 text-sm xl:gap-6'>
        <AdminNavLink
          to='/elections/$electionRoundId'
          title='Dashboard'
          activeOptions={{ exact: true }}
        />
        <AdminNavLink to='/elections/$electionRoundId/forms' title='Forms' />
        <AdminNavLink
          to='/elections/$electionRoundId/observers'
          title='Observers'
        />

        <AdminNavLink to='/elections/$electionRoundId/guides' title='Guides' />
        <AdminNavLink
          to='/elections/$electionRoundId/submissions'
          title='Submissions'
        />
        <AdminNavLink
          to='/elections/$electionRoundId/quick-reports'
          title='Quick reports'
        />
        <AdminNavLink
          to='/elections/$electionRoundId/incidents'
          title='Incident reports'
        />
      </nav>
    </div>
  )
}
