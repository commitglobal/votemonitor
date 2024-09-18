import { ZFormType } from '@/common/types';
import { FILTER_KEY, FILTER_LABEL } from '@/features/filtering/filtering-enums';
import { mapFormType } from '@/lib/utils';
import { FC } from 'react';
import { SelectFilter, SelectFilterOption } from '../../filtering/components/SelectFilter';
import { useFilteringContainer } from '../../filtering/hooks/useFilteringContainer';

const responseFormTypeOptions: SelectFilterOption[] = [
  {
    value: ZFormType.Values.Opening,
    label: mapFormType(ZFormType.Values.Opening),
  },

  {
    value: ZFormType.Values.Voting,
    label: mapFormType(ZFormType.Values.Voting),
  },

  {
    value: ZFormType.Values.ClosingAndCounting,
    label: mapFormType(ZFormType.Values.ClosingAndCounting),
  },
  {
    value: ZFormType.Values.Other,
    label: mapFormType(ZFormType.Values.Other),
  },
];

export const ResponseFormTypeFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onStatusChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormTypeFilter]: value });
  };

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FormTypeFilter]}
      onChange={onStatusChange}
      placeholder={FILTER_LABEL.FormTypeFilter}
      options={responseFormTypeOptions}
    />
  );
};
