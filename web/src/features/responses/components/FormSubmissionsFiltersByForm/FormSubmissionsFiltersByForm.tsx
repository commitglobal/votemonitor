import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsCompletionFilter } from '@/features/filtering/components/FormSubmissionsCompletionFilter';
import { FormSubmissionsFollowUpFilter } from '@/features/filtering/components/FormSubmissionsFollowUpFilter';
import { FormSubmissionsFormFilter } from '@/features/filtering/components/FormSubmissionsFormFilter';
import { FormSubmissionsFromDateFilter } from '@/features/filtering/components/FormSubmissionsFromDateFilter';
import { FormSubmissionsQuestionsAnsweredFilter } from '@/features/filtering/components/FormSubmissionsQuestionsAnsweredFilter';
import { FormSubmissionsToDateFilter } from '@/features/filtering/components/FormSubmissionsToDateFilter';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FC } from 'react';
import { FormSubmissionsFlaggedAnswersFilter } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsMediaFilesFilter } from '../../../filtering/components/FormSubmissionsMediaFilesFilter';
import { FormSubmissionsQuestionNotesFilter } from '../../../filtering/components/FormSubmissionsQuestionNotesFilter';

export const FormSubmissionsFiltersByForm: FC = () => {
  return (
    <FilteringContainer>
      <FormSubmissionsFormFilter />
      <FormSubmissionsCompletionFilter />
      <FormSubmissionsFlaggedAnswersFilter />
      <FormSubmissionsFollowUpFilter />
      <FormSubmissionsQuestionsAnsweredFilter />
      <FormSubmissionsQuestionNotesFilter />
      <FormSubmissionsMediaFilesFilter />
      <MonitoringObserverTagsSelect />
      <PollingStationsFilters />
      <FormSubmissionsFromDateFilter />
      <FormSubmissionsToDateFilter />
    </FilteringContainer>
  );
};
