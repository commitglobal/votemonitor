import { BinarySelectFilter } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC } from 'react';

export const FormSubmissionsFlaggedAnswersFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.HasFlaggedAnswers]: value });
  };

  return (
    <BinarySelectFilter
      value={(queryParams as any)[FILTER_KEY.HasFlaggedAnswers]}
      placeholder='Flagged answers'
      onChange={onChange}
    />
  );
};
