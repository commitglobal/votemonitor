import { Badge } from '@/components/ui/badge';
import { cn, mapNgoAdminStatus, mapNgoStatus } from '@/lib/utils';
import { FC } from 'react';
import { NGOStatus } from '../models/NGO';
import { NgoAdminStatus } from '../models/NgoAdmin';

interface NgoStatusBadgeProps {
  status: NGOStatus;
}

export const NgoStatusBadge: FC<NgoStatusBadgeProps> = ({ status }) => {
  return (
    <Badge
      className={cn('w-fit', {
        'text-green-600 bg-green-200': status === NGOStatus.Activated,
        'text-slate-700 bg-slate-200': status === NGOStatus.Deactivated,
      })}>
      {mapNgoStatus(status)}
    </Badge>
  );
};

interface NgoAdmintatusBadgeProps {
  status: NgoAdminStatus;
}

export const NgoAdminStatusBadge: FC<NgoAdmintatusBadgeProps> = ({ status }) => {
  return (
    <Badge
      className={cn('w-fit', {
        'text-green-600 bg-green-200': status === NgoAdminStatus.Active,
        'text-slate-700 bg-slate-200': status === NgoAdminStatus.Deactivated,
      })}>
      {mapNgoAdminStatus(status)}
    </Badge>
  );
};
