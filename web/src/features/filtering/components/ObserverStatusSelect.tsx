import { FC } from 'react';
import { useFilteringContainer } from '../hooks/useFilteringContainer';
import { GenericSelectFilter, GenericSelectFilterOption } from './GenericSelectFilter';

const observerStatusOptions: GenericSelectFilterOption[] = [
  {
    value: 'Active',
    label: 'Active',
  },

  {
    value: 'Pending',
    label: 'Pending',
  },

  {
    value: 'Suspended',
    label: 'Suspended',
  },
];

export const ObserverStatusSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onStatusChange = (value: string) => {
    navigateHandler({ status: value });
  };

  return (
    <GenericSelectFilter
      value={(queryParams as any).status}
      onChange={onStatusChange}
      placeholder='Observer status'
      options={observerStatusOptions}
    />
  );
};
