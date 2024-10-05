import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { SelectFilter } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC, useMemo } from 'react';
import { useFormSubmissionsFilters } from '../hooks/form-submissions-queries';

export const FormSubmissionsFormSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data } = useFormSubmissionsFilters(currentElectionRoundId);

  const options = useMemo(() => {
    return data?.formFilterOptions.map((f) => ({
      value: f.formId,
      label: `${f.formCode} - ${f.formName}`,
    })) ??[];
  }, [data]);


  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormId]: value });
  };

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FormId]}
      onChange={onChange}
      options={options}
      placeholder='Form'
    />
  );
};
