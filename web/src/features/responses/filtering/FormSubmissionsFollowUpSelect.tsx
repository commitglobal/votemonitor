import { FormSubmissionFollowUpStatus } from '@/common/types';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC } from 'react';
import { mapFormSubmissionFollowUpStatus } from '../utils/helpers';

export const FormSubmissionsFollowUpSelect: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormSubmissionFollowUpStatus]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: FormSubmissionFollowUpStatus.NotApplicable,
      label: mapFormSubmissionFollowUpStatus(FormSubmissionFollowUpStatus.NotApplicable)
    },

    {
      value: FormSubmissionFollowUpStatus.NeedsFollowUp,
      label: mapFormSubmissionFollowUpStatus(FormSubmissionFollowUpStatus.NeedsFollowUp)
    },
    {
      value: FormSubmissionFollowUpStatus.Resolved,
      label: mapFormSubmissionFollowUpStatus(FormSubmissionFollowUpStatus.Resolved)
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FormSubmissionFollowUpStatus]}
      onChange={onChange}
      options={options}
      placeholder='Follow-up status'
    />
  );
};
