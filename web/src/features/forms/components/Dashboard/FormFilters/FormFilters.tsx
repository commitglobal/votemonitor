import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FC } from 'react';
import { FormTypeFilter } from '../../../../filtering/components/FormTypeFilter';
import { FormStatusFilter } from '../../../../filtering/components/FormStatusFilter';

export const FormFilters: FC = () => {
  return (
    <FilteringContainer>
      <FormTypeFilter />
      <FormStatusFilter />
    </FilteringContainer>
  );
};
