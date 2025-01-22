import { ElectionRoundStatus } from '@/common/types';
import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';

export interface ElectionRoundStatusBadgeProps {
  status: ElectionRoundStatus;
}

function ElectionRoundStatusBadge({ status }: ElectionRoundStatusBadgeProps) {
  return (
    <Badge
      className={cn('w-fit', {
        'text-green-600 bg-green-200': status === ElectionRoundStatus.Started,
        'text-yellow-600 bg-yellow-200': status === ElectionRoundStatus.Archived,
        'text-slate-700 bg-slate-200': status === ElectionRoundStatus.NotStarted,
      })}>
      {status === ElectionRoundStatus.Started
        ? 'Started'
        : status === ElectionRoundStatus.Archived
          ? 'Archived'
          : 'Not Started'}
    </Badge>
  );
}

export default ElectionRoundStatusBadge;
