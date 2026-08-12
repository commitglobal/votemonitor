import { MonitoringObserverStatus } from '@/types/monitoring-observer'
import { cn } from '@/lib/utils'
import { Badge } from '../ui/badge'

/**
 * The status is written out rather than run through `mapMonitoringObserverStatus`:
 * that helper looks up `observers.status.*`, and those keys are not in the
 * locale files yet, so it would print the key instead of a word.
 */
export default function MonitoringObserverStatusBadge({
  status,
}: {
  status: MonitoringObserverStatus
}) {
  return (
    <Badge
      className={cn('w-fit', {
        'bg-green-200 text-green-600':
          status === MonitoringObserverStatus.Active,
        'bg-yellow-200 text-yellow-600':
          status === MonitoringObserverStatus.Pending,
        'bg-slate-200 text-slate-700':
          status === MonitoringObserverStatus.Suspended,
      })}
    >
      {status}
    </Badge>
  )
}
