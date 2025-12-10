import { ElectionRoundStatus } from '@/types/election'
import { mapElectionRoundStatus } from '@/lib/i18n'
import { cn } from '@/lib/utils'
import { Badge } from '../ui/badge'

export default function ElectionRoundStatusBadge({
  status,
}: {
  status: ElectionRoundStatus
}) {
  return (
    <Badge
      className={cn('w-fit', {
        'bg-green-200 text-green-600': status === ElectionRoundStatus.Started,
        'bg-yellow-200 text-yellow-600':
          status === ElectionRoundStatus.Archived,
        'bg-slate-200 text-slate-700':
          status === ElectionRoundStatus.NotStarted,
      })}
    >
      {mapElectionRoundStatus(status)}
    </Badge>
  )
}
