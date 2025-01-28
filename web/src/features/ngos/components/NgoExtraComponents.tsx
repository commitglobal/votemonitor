import { usePrevSearch } from '@/common/prev-search-store';
import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import { Link } from '@tanstack/react-router';
import { FC, useEffect, useState } from 'react';

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

interface NgoBreadcrumbsProps {
  ngoData: {
    id: string;
    name: string;
  };
  adminData?: {
    id: string;
    name: string;
  };
  tab?: string;
}

export const NgoBreadcrumbs: FC<NgoBreadcrumbsProps> = ({ ngoData, adminData, tab }) => {
  const [ngoName, setNgoName] = useState<null | string>(null);

  useEffect(() => {
    if (!ngoName && ngoData.name !== undefined) setNgoName(ngoData.name);
  }, [ngoData.name]);

  return (
    <div className='breadcrumbs flex flex-row gap-2 mb-4'>
      <Link className='crumb' to='/ngos' preload='intent'>
        ngos
      </Link>
      {ngoName && (
        <Link
          className='crumb'
          to='/ngos/view/$ngoId/$tab'
          params={{ tab: 'details', ngoId: ngoData.id }}
          preload='intent'>
          {ngoName}
        </Link>
      )}
      <Link
        className='crumb'
        to='/ngos/view/$ngoId/$tab'
        params={{ tab: tab ?? 'admins', ngoId: ngoData.id }}
        preload='intent'>
        {tab ?? 'admins'}
      </Link>
      <Link
        className='crumb'
        to='/ngos/admin/$ngoId/$adminId/view'
        params={{ ngoId: ngoData.id, adminId: adminData?.id as string }}
        preload='intent'>
        {adminData?.name}
      </Link>
    </div>
  );
};
