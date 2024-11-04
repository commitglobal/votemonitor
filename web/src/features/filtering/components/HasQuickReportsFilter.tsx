import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC } from 'react';
import { BinarySelectFilter } from './SelectFilter';

export const HasQuickReportsFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.HasQuickReports]: value });
  };

  return (
    <BinarySelectFilter
      value={(queryParams as any)[FILTER_KEY.HasQuickReports]}
      placeholder='Has quick reports'
      onChange={onChange}
    />
  );
};
