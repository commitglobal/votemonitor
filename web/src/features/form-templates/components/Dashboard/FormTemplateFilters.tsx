import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormTemplateStatusFilter } from '@/features/filtering/components/FormTemplateStatusFilter';
import { FormTemplateTypeFilter } from '@/features/filtering/components/FormTemplateTypeFilter';
import { FC } from 'react';

export const FormTemplateFilters: FC = () => {
  return (
    <FilteringContainer>
      <FormTemplateTypeFilter />
      <FormTemplateStatusFilter />
    </FilteringContainer>
  );
};
