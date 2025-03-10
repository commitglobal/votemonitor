import { usePrevSearch } from '@/common/prev-search-store';
import { Link, useRouter } from '@tanstack/react-router';
import { FC } from 'react';

type CustomAlias = {
  param: string;
  alias: string;
};

interface BreadcrumbsWithAliasesProps {
  customAliases: CustomAlias[];
}

const DEFAULT_ALIASES = new Map<string, string>([
  ['observers', 'Observers'],
  ['form-templates', 'Form templates'],
]);

const INTERNAL_ALIASES = new Map<string, string>(DEFAULT_ALIASES);

export const BreadcrumbsWithAliases: FC<BreadcrumbsWithAliasesProps> = ({ customAliases }) => {
  const { latestLocation } = useRouter();
  const prevSearch = usePrevSearch();

  customAliases.forEach((aliasData) => INTERNAL_ALIASES.set(aliasData.param, aliasData.alias));

  let currentLink: string = '';

  const crumbs = latestLocation.pathname
    .split('/')
    .filter((crumb) => crumb !== '')
    .map((crumb) => {
      currentLink += `/${crumb}`;

      return (
        <Link className='crumb' key={crumb} search={prevSearch} to={currentLink} preload='intent'>
          {INTERNAL_ALIASES.get(crumb) ?? crumb}
        </Link>
      );
    });

  return <>{crumbs.length > 1 ? <div className='breadcrumbs flex flex-row gap-2 mb-4'>{crumbs}</div> : ''}</>;
};
