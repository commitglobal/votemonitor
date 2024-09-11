import { usePrevSearch } from '@/common/prev-search-store';
import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import { Link } from '@tanstack/react-router';
import { FC } from 'react';

export const MonitorObserverBackButton: FC = () => {
  const prevSearch = usePrevSearch();
  return (
    <Link search={prevSearch} to='/monitoring-observers/$tab/' params={{ tab: 'list' }} preload='intent'>
      <BackButtonIcon />
    </Link>
  );
};
