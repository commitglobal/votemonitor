import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { ObserverStatusSelect } from '@/features/filtering/components/ObserverStatusSelect';
import { FC } from 'react';



export const MonitoringObserversListFilters: FC = () => {
  return <FilteringContainer>
    <ObserverStatusSelect />
  </FilteringContainer>
};
