import { Badge } from '@/components/ui/badge';
import { FC } from 'react';
import { NGOStatus } from '../models/NGO';
import { NgoAdminStatus } from '../models/NgoAdmin';

interface NgoStatusBadgeProps {
  status: NGOStatus;
}

export const NgoStatusBadge: FC<NgoStatusBadgeProps> = ({ status }) => {
  let className = '';

  switch (status) {
    case NGOStatus.Activated:
      className = 'badge-Active';
      break;

    case NGOStatus.Pending:
      className = 'badge-Pending';
      break;

    case NGOStatus.Deactivated:
      className = 'badge-Suspended';
      break;

    default:
      break;
  }

  return <Badge className={className}>{status}</Badge>;
};

interface NgoAdmintatusBadgeProps {
  status: NgoAdminStatus;
}

export const NgoAdminStatusBadge: FC<NgoAdmintatusBadgeProps> = ({ status }) => {
  let className = '';

  switch (status) {
    case NgoAdminStatus.Active:
      className = 'badge-Active';
      break;

    case NgoAdminStatus.Deactivated:
      className = 'badge-Suspended';
      break;

    default:
      break;
  }

  return <Badge className={className}>{status}</Badge>;
};
