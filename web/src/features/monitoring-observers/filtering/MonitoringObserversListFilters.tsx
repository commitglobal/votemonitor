import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { MonitoringObserverStatusSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverStatusSelect';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FC } from 'react';

export const MonitoringObserversListFilters: FC = () => {
  return (
    <FilteringContainer>
      <MonitoringObserverStatusSelect />
      <MonitoringObserverTagsSelect />
    </FilteringContainer>
  );
};
