import { Badge } from '@/components/ui/badge';
import { FC } from 'react';
import { NGOStatus } from '../models/NGO';

interface NGOStatusBadgeProps {
  status: NGOStatus;
}

export const NGOStatusBadge: FC<NGOStatusBadgeProps> = ({ status }) => {
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
