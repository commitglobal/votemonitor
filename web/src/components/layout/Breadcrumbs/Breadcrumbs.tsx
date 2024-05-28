import { usePrevSearch } from '@/common/prev-search-store';
import type { FunctionComponent } from '@/common/types';
import { Link, useRouter } from '@tanstack/react-router';

const Breadcrumbs = (): FunctionComponent => {
  const { latestLocation } = useRouter();
  const prevSearch = usePrevSearch();

  let currentLink: string = '';

  const crumbs = latestLocation.pathname
    .split('/')
    .filter((crumb) => crumb !== '')
    .map((crumb) => {
      currentLink += `/${crumb}`;

      return (
        <Link className='crumb' key={crumb} search={prevSearch} to={currentLink} preload='intent'>
          {crumb}
        </Link>
      );
    });

  return <>{crumbs.length > 1 ? <div className='breadcrumbs flex flex-row gap-2 mb-4'>{crumbs}</div> : ''}</>;
};

export default Breadcrumbs;
