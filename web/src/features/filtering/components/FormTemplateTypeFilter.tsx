import { FormType } from '@/common/types';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { mapFormType } from '@/lib/utils';
import { FC, useMemo } from 'react';

export const FormTemplateTypeFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormTemplateTypeFilter]: value });
  };

  const selectOptions = useMemo(() => {
    const options: SelectFilterOption[] = [
      {
        value: FormType.Opening,
        label: mapFormType(FormType.Opening),
      },
      {
        value: FormType.Voting,
        label: mapFormType(FormType.Voting),
      },
      {
        value: FormType.ClosingAndCounting,
        label: mapFormType(FormType.ClosingAndCounting),
      },
      {
        value: FormType.PSI,
        label: mapFormType(FormType.PSI),
      },
      {
        value: FormType.CitizenReporting,
        label: mapFormType(FormType.CitizenReporting),
      },
      {
        value: FormType.IncidentReporting,
        label: mapFormType(FormType.IncidentReporting),
      },
      {
        value: FormType.Other,
        label: mapFormType(FormType.Other),
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
