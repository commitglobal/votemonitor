import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { MonitoringObserverStatus } from '@/features/monitoring-observers/models/monitoring-observer';
import { FC } from 'react';
import { SelectFilter, SelectFilterOption } from '../../filtering/components/SelectFilter';
import { useFilteringContainer } from '../../filtering/hooks/useFilteringContainer';

const monitoringObserverStatusOptions: SelectFilterOption[] = [
  {
    value: MonitoringObserverStatus.Active,
    label: MonitoringObserverStatus.Active,
  },

  {
    value: MonitoringObserverStatus.Pending,
    label: MonitoringObserverStatus.Pending,
  },

  {
    value: MonitoringObserverStatus.Suspended,
    label: MonitoringObserverStatus.Suspended,
  },
];

export const MonitoringObserverStatusSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onStatusChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.MonitoringObserverStatus]: value });
  };

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.MonitoringObserverStatus]}
      onChange={onStatusChange}
      placeholder='Observer status'
      options={monitoringObserverStatusOptions}
    />
  );
};
