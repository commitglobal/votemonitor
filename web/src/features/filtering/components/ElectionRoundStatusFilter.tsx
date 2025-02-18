import { ElectionRoundStatus } from '@/common/types';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { mapElectionRoundStatus } from '@/lib/utils';
import { FC } from 'react';

export const ElectionRoundStatusFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.ElectionRoundStatusFilter]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: ElectionRoundStatus.NotStarted,
      label: mapElectionRoundStatus(ElectionRoundStatus.NotStarted),
    },

    {
      value: ElectionRoundStatus.Started,
      label: mapElectionRoundStatus(ElectionRoundStatus.Started),
    },

    {
      value: ElectionRoundStatus.Archived,
      label: mapElectionRoundStatus(ElectionRoundStatus.Archived),
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.ElectionRoundStatusFilter]}
      onChange={onChange}
      options={options}
      placeholder='Election round status'
    />
  );
};
