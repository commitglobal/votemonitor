import { ElectionRoundStatus, FormStatus } from '@/common/types';
import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';

export interface FormStatusBadgeProps {
  status: FormStatus;
}

function FormStatusBadge({ status }: FormStatusBadgeProps) {
  return (
    <Badge
      className={cn({
        'text-slate-700 bg-slate-200': status === FormStatus.Drafted,
        'text-green-600 bg-green-200': status === FormStatus.Published,
        'text-yellow-600 bg-yellow-200': status === FormStatus.Obsolete,
      })}>
      {status}
    </Badge>
  );
}

export default FormStatusBadge;
