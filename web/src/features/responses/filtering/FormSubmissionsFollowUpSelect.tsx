import { FollowUpStatus } from '@/common/types';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC } from 'react';

export const FormSubmissionsFollowUpSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FollowUpStatus]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: FollowUpStatus.NotApplicable,
      label: 'Not applicable',
    },

    {
      value: FollowUpStatus.NeedsFollowUp,
      label: 'Needs follow up',
    },
    {
      value: FollowUpStatus.Resolved,
      label: 'Resolved',
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FollowUpStatus]}
      onChange={onChange}
      options={options}
      placeholder='Follow-up status'
    />
  );
};
