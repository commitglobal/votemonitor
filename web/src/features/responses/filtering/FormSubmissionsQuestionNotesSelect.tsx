import { BinarySelectFilter } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC } from 'react';

export const FormSubmissionsQuestionNotesSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.HasNotes]: value });
  };

  return (
    <BinarySelectFilter
      value={(queryParams as any)[FILTER_KEY.HasNotes]}
      placeholder='Question notes'
      onChange={onChange}
    />
  );
};
