import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC } from 'react';
import { NGOStatus } from '../../models/NGO';

const ngoStatusOptions: SelectFilterOption[] = [
  {
    value: NGOStatus.Activated,
    label: NGOStatus.Activated,
  },

  {
    value: NGOStatus.Deactivated,
    label: NGOStatus.Deactivated,
  },
];

export const NGOStatusSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onStatusChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.Status]: value });
  };

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.Status]}
      onChange={onStatusChange}
      placeholder='NGO status'
      options={ngoStatusOptions}
    />
  );
};
