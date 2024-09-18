import { FILTER_KEY, FILTER_LABEL } from '@/features/filtering/filtering-enums';
import { FC } from 'react';
import { BinarySelectFilter } from '../../filtering/components/SelectFilter';
import { useFilteringContainer } from '../../filtering/hooks/useFilteringContainer';

export const ResponseFormFlaggedAnswersFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onStatusChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormTypeFilter]: value });
  };

  return (
    <BinarySelectFilter
      value={(queryParams as any)[FILTER_KEY.FormTypeFilter]}
      onChange={onStatusChange}
      placeholder={FILTER_LABEL.FormTypeFilter}
    />
  );
};
