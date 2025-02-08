import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FormTemplateStatus } from '@/features/form-templates/models';
import { mapFormTemplateStatus } from '@/lib/utils';
import { FC } from 'react';

export const FormTemplateStatusFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormTemplateStatusFilter]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: FormTemplateStatus.Drafted,
      label: mapFormTemplateStatus(FormTemplateStatus.Drafted),
    },

    {
      value: FormTemplateStatus.Published,
      label: mapFormTemplateStatus(FormTemplateStatus.Published),
    },

    {
      value: FormTemplateStatus.Obsolete,
      label: mapFormTemplateStatus(FormTemplateStatus.Obsolete),
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FormTemplateStatusFilter]}
      onChange={onChange}
      options={options}
      placeholder='Form status'
    />
  );
};
