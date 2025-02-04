import { Button } from '@/components/ui/button';
import { AuthContext } from '@/context/auth.context';
import { FC, ReactNode, useContext } from 'react';
import { useFilteringContainer } from '../hooks/useFilteringContainer';
import { NgoAdminActiveFilters } from './NgoAdminActiveFilters';
import { PlatformAdminActiveFilters } from './PlatformAdminActiveFilters';

interface FilteringContainerProps {
  children?: ReactNode;
}

export const FilteringContainer: FC<FilteringContainerProps> = ({ children }) => {
  const { filteringIsActive, queryParams, resetFilters } = useFilteringContainer();
  const { userRole } = useContext(AuthContext);

  return (
    <div className='grid items-center grid-cols-6 gap-4'>
      {children}
      <Button title='Reset filters' disabled={!filteringIsActive} onClick={resetFilters} variant='ghost-primary'>
        Reset filters
      </Button>
      {filteringIsActive && userRole === 'NgoAdmin' ? (
        <NgoAdminActiveFilters queryParams={queryParams} />
      ) : (
        <PlatformAdminActiveFilters queryParams={queryParams} />
      )}
    </div>
  );
};
