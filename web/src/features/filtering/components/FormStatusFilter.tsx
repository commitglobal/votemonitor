import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { mapFormStatus } from '@/lib/utils';
import { FC } from 'react';
import { FormStatus } from '../../forms/models/form';

export const FormStatusFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormStatusFilter]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: FormStatus.Drafted,
      label: mapFormStatus(FormStatus.Drafted),
    },

    {
      value: FormStatus.Published,
      label: mapFormStatus(FormStatus.Published),
    },

    {
      value: FormStatus.Obsolete,
      label: mapFormStatus(FormStatus.Obsolete),
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FormStatusFilter]}
      onChange={onChange}
      options={options}
      placeholder='Form status'
    />
  );
};
