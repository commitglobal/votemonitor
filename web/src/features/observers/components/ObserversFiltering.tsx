import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { FC } from 'react';
import { SelectFilter, SelectFilterOption } from '../../filtering/components/SelectFilter';
import { useFilteringContainer } from '../../filtering/hooks/useFilteringContainer';
import { ObserverStatus } from '../models/observer';

const observerStatusOptions: SelectFilterOption[] = [
  {
    value: ObserverStatus.Active,
    label: ObserverStatus.Active,
  },

  {
    value: ObserverStatus.Pending,
    label: ObserverStatus.Pending,
  },

  {
    value: ObserverStatus.Deactivated,
    label: ObserverStatus.Deactivated,
  },
];

const ObserverStatusSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onStatusChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.ObserverStatus]: value });
  };

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.ObserverStatus]}
      onChange={onStatusChange}
      placeholder='Observer status'
      options={observerStatusOptions}
    />
  );
};

export const ObserversListFilters: FC = () => {
  return (
    <FilteringContainer>
      <ObserverStatusSelect />
    </FilteringContainer>
  );
};
