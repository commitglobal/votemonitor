import { usePrevSearch } from '@/common/prev-search-store';
import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import { Link } from '@tanstack/react-router';
import { FC } from 'react';

interface NgoBackButtonProps {
  ngoId?: string;
}

export const NgoBackButton: FC<NgoBackButtonProps> = ({ ngoId }) => {
  const prevSearch = usePrevSearch();
  const destination = ngoId ? '/ngos/view/$ngoId/$tab' : '/ngos';
  const linkParams = ngoId ? { ngoId, tab: 'admins' } : {};

  return (
    <Link title='Go back' search={prevSearch as any} to={destination} params={linkParams} preload='intent'>
      <BackButtonIcon />
    </Link>
  );
};
