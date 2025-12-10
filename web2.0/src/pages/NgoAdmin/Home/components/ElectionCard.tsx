import { Link } from '@tanstack/react-router'
import type { MonitoredElection } from '@/types/monitored-election'
import { CalendarDays, Eye, Shield, Users } from 'lucide-react'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import ElectionRoundStatusBadge from '@/components/badges/election-round-status-badge'
import { CountryFlag } from '@/components/country-flag'

interface ElectionCardProps {
  electionRound: MonitoredElection
}

export function ElectionRoundCard({ electionRound }: ElectionCardProps) {
  const showCoalition =
    !electionRound.coalitionId || electionRound.coalitionId === ''

  return (
    <Card>
      <CardHeader>
        <CardTitle>{electionRound.title}</CardTitle>
        <CardDescription>{electionRound.englishTitle}</CardDescription>
        <CardAction>
          <Button variant='link' asChild>
            <Link
              to='/elections/$electionRoundId'
              params={{ electionRoundId: electionRound.id }}
            >
              <CountryFlag
                code={electionRound.countryIso2}
                className='size-10 rounded-lg'
              />
            </Link>
          </Button>
        </CardAction>
      </CardHeader>
      <CardContent>
        <div className='space-y-2'>
          <div className='flex items-center gap-2 text-sm'>
            <CalendarDays className='text-muted-foreground h-4 w-4' />
            <span>{electionRound.startDate}</span>
          </div>

          <div className='flex flex-wrap gap-2'>
            <ElectionRoundStatusBadge status={electionRound.status} />
            {showCoalition && (
              <div className='flex items-center gap-2 text-sm'>
                <Users className='text-muted-foreground h-4 w-4' />
                <span className='font-medium'>Coalition:</span>
                <span>{electionRound.coalitionName}</span>
              </div>
            )}
            {electionRound.isCoalitionLeader && (
              <Badge variant='secondary' className='text-xs'>
                <Shield className='mr-1 h-3 w-3' />
                Coalition Leader
              </Badge>
            )}

            {electionRound.isMonitoringNgoForCitizenReporting && (
              <Badge variant='outline' className='text-xs'>
                Citizen Reporting
              </Badge>
            )}
          </div>
        </div>
      </CardContent>
      <CardFooter className='flex justify-end'>
        <Button variant='link' asChild>
          <Link
            to='/elections/$electionRoundId'
            params={{ electionRoundId: electionRound.id }}
          >
            <Eye className='mr-2 h-4 w-4' />
            View Details
          </Link>
        </Button>
      </CardFooter>
    </Card>
  )
}
