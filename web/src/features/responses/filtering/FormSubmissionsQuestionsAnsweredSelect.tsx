import { QuestionsAnswered } from '@/common/types';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC } from 'react';

export const FormSubmissionsQuestiosAnsweredSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.QuestionsAnswered]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: QuestionsAnswered.None,
      label: QuestionsAnswered.None,
    },

    {
      value: QuestionsAnswered.Some,
      label: QuestionsAnswered.Some,
    },
    {
      value: QuestionsAnswered.All,
      label: QuestionsAnswered.All,
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.QuestionsAnswered]}
      onChange={onChange}
      options={options}
      placeholder='Questions answered'
    />
  );
};
