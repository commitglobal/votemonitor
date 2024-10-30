import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsFollowUpFilter } from '@/features/filtering/components/FormSubmissionsFollowUpFilter';
import { FormSubmissionsFormFilter } from '@/features/filtering/components/FormSubmissionsFormFilter';
import { FormSubmissionsQuestionsAnsweredFilter } from '@/features/filtering/components/FormSubmissionsQuestionsAnsweredFilter';
import { FC } from 'react';
import { FormSubmissionsFlaggedAnswersFilter } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsMediaFilesFilter } from '../../../filtering/components/FormSubmissionsMediaFilesFilter';
import { FormSubmissionsQuestionNotesFilter } from '../../../filtering/components/FormSubmissionsQuestionNotesFilter';

export const CitizenReportsFiltersByForm: FC = () => {
  return (
    <FilteringContainer>
      <FormSubmissionsFormFilter />
      <FormSubmissionsFlaggedAnswersFilter />
      <FormSubmissionsFollowUpFilter />
      <FormSubmissionsQuestionsAnsweredFilter />
      <FormSubmissionsQuestionNotesFilter />
      <FormSubmissionsMediaFilesFilter />
      <PollingStationsFilters />
    </FilteringContainer>
  );
};
