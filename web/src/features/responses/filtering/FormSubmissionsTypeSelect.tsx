import { ZFormType } from '@/common/types';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { mapFormType } from '@/lib/utils';
import { FC } from 'react';

export const FormSubmissionsTypeSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormTypeFilter]: value });
  };
  const options: SelectFilterOption[] = [
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
      value: ZFormType.Values.PSI,
      label: mapFormType(ZFormType.Values.PSI),
    },

    {
      value: ZFormType.Values.IncidentReporting,
      label: mapFormType(ZFormType.Values.IncidentReporting),
    },

    {
      value: ZFormType.Values.Other,
      label: mapFormType(ZFormType.Values.Other),
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FormTypeFilter]}
      onChange={onChange}
      options={options}
      placeholder='Form type'
    />
  );
};
