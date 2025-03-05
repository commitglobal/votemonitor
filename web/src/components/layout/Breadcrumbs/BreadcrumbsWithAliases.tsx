import { usePrevSearch } from '@/common/prev-search-store';
import { Link, useRouter } from '@tanstack/react-router';
import { FC } from 'react';

type CustomAlias = {
  param: string;
  alias: string;
};

interface BreadcrumbsWithAliasesProps {
  customAliases?: CustomAlias[];
}

export const BreadcrumbsWithAliases: FC<BreadcrumbsWithAliasesProps> = ({ customAliases }) => {
  const { latestLocation } = useRouter();
  const prevSearch = usePrevSearch();

  const DEFAULT_ALIASES = new Map<string, string>([['observers', 'Observers']]);

  let currentLink: string = '';

  const crumbs = latestLocation.pathname
    .split('/')
    .filter((crumb) => crumb !== '')
    .map((crumb) => {
      let crumbWithAlias = undefined;
      currentLink += `/${crumb}`;

      if (DEFAULT_ALIASES.has(crumb)) crumbWithAlias = DEFAULT_ALIASES.get(crumb);

      if (customAliases) {
        const customAliasResult = customAliases.find((aliasObj) => aliasObj.param === crumb);

        if (customAliasResult) crumbWithAlias = customAliasResult.alias;
      }

      return (
        <Link className='crumb' key={crumb} search={prevSearch} to={currentLink} preload='intent'>
          {crumbWithAlias ?? crumb}
        </Link>
      );
    });

  return <>{crumbs.length > 1 ? <div className='breadcrumbs flex flex-row gap-2 mb-4'>{crumbs}</div> : ''}</>;
};
