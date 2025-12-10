import { QuickReportFollowUpStatus } from '@/types/quick-reports'
import { mapQuickReportFollowUpStatus } from '@/lib/i18n'
import { cn } from '@/lib/utils'
import { Badge } from '../ui/badge'

export default function QuickReportFollowUpStatusBadge({
  status,
}: {
  status: QuickReportFollowUpStatus
}) {
  return (
    <Badge
      className={cn({
        'bg-slate-200 text-slate-700':
          status === QuickReportFollowUpStatus.NotApplicable,
        'bg-red-200 text-red-600':
          status === QuickReportFollowUpStatus.NeedsFollowUp,
        'bg-green-200 text-green-600':
          status === QuickReportFollowUpStatus.Resolved,
      })}
    >
      {mapQuickReportFollowUpStatus(status)}
    </Badge>
  )
}
