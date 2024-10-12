import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FC } from 'react';
import { FormTypeSelect } from '../../filtering/FormTypeSelect';
import { FormStatusSelect } from '../../filtering/FormStatusSelect';

export const FormFilters: FC = () => {
  return (
    <FilteringContainer>
      <FormTypeSelect />
      <FormStatusSelect />
    </FilteringContainer>
  );
};
