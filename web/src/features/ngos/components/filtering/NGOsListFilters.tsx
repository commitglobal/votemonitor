import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FC } from 'react';
import { NGOStatusSelect } from './NGOStatusSelect';

export const NGOsListFilters: FC = () => {
  return (
    <FilteringContainer>
      <NGOStatusSelect />
    </FilteringContainer>
  );
};
