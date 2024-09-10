import { Button } from '@/components/ui/button';
import { FC, ReactNode } from 'react';
import { useFilteringContainer } from '../hooks/useFilteringContainer';

interface FilteringContainerProps {
  children?: ReactNode;
}

export const FilteringContainer: FC<FilteringContainerProps> = ({ children }) => {
  const { filteringIsActive, resetFilters } = useFilteringContainer();
  return (
    <div className='grid items-center grid-cols-6 gap-4'>
      {children}
      <Button title='Reset filters' disabled={!filteringIsActive} onClick={resetFilters} variant='ghost-primary'>
        Reset filters
      </Button>
    </div>
  );
};
