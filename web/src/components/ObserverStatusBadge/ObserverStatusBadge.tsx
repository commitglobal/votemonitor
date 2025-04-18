import { ObserverStatus } from '@/common/types';
import { Badge } from '@/components/ui/badge';
import { cn, mapObserverStatus } from '@/lib/utils';

export interface ElectionRoundStatusBadgeProps {
  status: ObserverStatus;
}

function ObserverStatusBadge({ status }: ElectionRoundStatusBadgeProps) {
  return (
    <Badge
      className={cn('w-fit', {
        'text-green-600 bg-green-200': status === ObserverStatus.Active,
        'text-yellow-600 bg-yellow-200': status === ObserverStatus.Pending,
        'text-slate-700 bg-slate-200': status === ObserverStatus.Deactivated,
      })}>
      ObserverStatus
      {mapObserverStatus(status)}
    </Badge>
  );
}

export default ObserverStatusBadge;
