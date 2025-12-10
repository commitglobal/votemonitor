import { FormSubmissionFollowUpStatus } from '@/types/forms-submission'
import { mapFormSubmissionFollowUpStatus } from '@/lib/i18n'
import { cn } from '@/lib/utils'
import { Badge } from '../ui/badge'

export default function FormSubmissionFollowUpStatusBadge({
  followUpStatus,
}: {
  followUpStatus: FormSubmissionFollowUpStatus
}) {
  return (
    <Badge
      className={cn({
        'bg-slate-200 text-slate-700':
          followUpStatus === FormSubmissionFollowUpStatus.NotApplicable,
        'bg-red-200 text-red-600':
          followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp,
        'bg-green-200 text-green-600':
          followUpStatus === FormSubmissionFollowUpStatus.Resolved,
      })}
    >
      {mapFormSubmissionFollowUpStatus(followUpStatus)}
    </Badge>
  )
}
