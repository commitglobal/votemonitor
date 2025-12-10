import { FormStatus } from '@/types/form'
import { mapFormStatus } from '@/lib/i18n'
import { cn } from '@/lib/utils'
import { Badge } from '../ui/badge'

export default function FormStatusBadge({
  formStatus,
}: {
  formStatus: FormStatus
}) {
  return (
    <Badge
      className={cn({
        'bg-slate-200 text-slate-700': formStatus === FormStatus.Drafted,
        'bg-green-200 text-green-600': formStatus === FormStatus.Published,
        'bg-orange-200 text-orange-600': formStatus === FormStatus.Drafted,
      })}
    >
      {mapFormStatus(formStatus)}
    </Badge>
  )
}
