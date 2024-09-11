import { usePrevSearch } from '@/common/prev-search-store';
import type { FunctionComponent } from '@/common/types';
import { Link, useRouter } from '@tanstack/react-router';
import { FC } from 'react';

export const BackButtonIcon: FC = () => {
  return (
    <svg xmlns='http://www.w3.org/2000/svg' width='30' height='30' viewBox='0 0 30 30' fill='none'>
      <path
        fillRule='evenodd'
        clipRule='evenodd'
        d='M19.0607 7.93934C19.6464 8.52513 19.6464 9.47487 19.0607 10.0607L14.1213 15L19.0607 19.9393C19.6464 20.5251 19.6464 21.4749 19.0607 22.0607C18.4749 22.6464 17.5251 22.6464 16.9393 22.0607L10.9393 16.0607C10.3536 15.4749 10.3536 14.5251 10.9393 13.9393L16.9393 7.93934C17.5251 7.35355 18.4749 7.35355 19.0607 7.93934Z'
        fill='#7833B3'
      />
    </svg>
  );
};

const BackButton = (): FunctionComponent => {
  const { latestLocation } = useRouter();
  const prevSearch = usePrevSearch();
  const links = latestLocation.pathname.split('/').filter((crumb: string) => crumb !== '');

  if (links.length <= 1) return <></>;

  return (
    <Link search={prevSearch} to='../' preload='intent'>
      <BackButtonIcon />
    </Link>
  );
};

export default BackButton;
