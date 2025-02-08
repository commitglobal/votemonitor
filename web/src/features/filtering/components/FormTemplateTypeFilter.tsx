import { FormTemplateType } from '@/common/types';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { mapFormTemplateType } from '@/lib/utils';
import { FC, useMemo } from 'react';

export const FormTemplateTypeFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormTemplateTypeFilter]: value });
  };

  const selectOptions = useMemo(() => {
    const options: SelectFilterOption[] = [
      {
        value: FormTemplateType.Opening,
        label: mapFormTemplateType(FormTemplateType.Opening),
      },
      {
        value: FormTemplateType.Voting,
        label: mapFormTemplateType(FormTemplateType.Voting),
      },
      {
        value: FormTemplateType.ClosingAndCounting,
        label: mapFormTemplateType(FormTemplateType.ClosingAndCounting),
      },
      {
        value: FormTemplateType.PSI,
        label: mapFormTemplateType(FormTemplateType.PSI),
      },
      {
        value: FormTemplateType.CitizenReporting,
        label: mapFormTemplateType(FormTemplateType.CitizenReporting),
      },
      {
        value: FormTemplateType.IncidentReporting,
        label: mapFormTemplateType(FormTemplateType.IncidentReporting),
      },
      {
        value: FormTemplateType.Other,
        label: mapFormTemplateType(FormTemplateType.Other),
      },
    ];

    return options;
  }, []);

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FormTemplateTypeFilter]}
      onChange={onChange}
      options={selectOptions}
      placeholder='Form template type'
    />
  );
};
