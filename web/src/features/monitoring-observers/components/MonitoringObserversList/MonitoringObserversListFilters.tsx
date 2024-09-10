import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { ObserverStatusSelect } from '@/features/filtering/components/ObserverStatusSelect';
import { ObserverTagsSelect } from '@/features/filtering/components/ObserverTagsSelect';
import { FC } from 'react';

export const MonitoringObserversListFilters: FC = () => {
  return (
    <FilteringContainer>
      <ObserverStatusSelect />
      <ObserverTagsSelect />
    </FilteringContainer>
  );
};
