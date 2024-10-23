import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsCompletionFilter } from '@/features/filtering/components/FormSubmissionsCompletionFilter';
import { FormSubmissionsFollowUpSelect } from '@/features/filtering/components/FormSubmissionsFollowUpFilter';
import { FormSubmissionsFormSelect } from '@/features/filtering/components/FormSubmissionsFormFilter';
import { FormSubmissionsFromDateFilter } from '@/features/filtering/components/FormSubmissionsFromDateFilter';
import { FormSubmissionsQuestionsAnsweredFilter } from '@/features/filtering/components/FormSubmissionsQuestionsAnsweredFilter';
import { FormSubmissionsToDateFilter } from '@/features/filtering/components/FormSubmissionsToDateFilter';
import { FormTypeFilter } from '@/features/filtering/components/FormTypeFilter';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FC } from 'react';
import { FormSubmissionsFlaggedAnswersSelect } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsMediaFilesSelect } from '../../../filtering/components/FormSubmissionsMediaFilesFilter';
import { FormSubmissionsQuestionNotesSelect } from '../../../filtering/components/FormSubmissionsQuestionNotesFilter';

export const FormSubmissionsFiltersByForm: FC = () => {
  return (
    <FilteringContainer>
      <FormSubmissionsFormSelect />
      <FormSubmissionsCompletionFilter />
      <FormSubmissionsFlaggedAnswersSelect />
      <FormSubmissionsFollowUpSelect />
      <FormSubmissionsQuestionsAnsweredFilter />
      <FormSubmissionsQuestionNotesSelect />
      <FormSubmissionsMediaFilesSelect />
      <MonitoringObserverTagsSelect isFilteringFormSubmissions />
      <PollingStationsFilters />
      <FormSubmissionsFromDateFilter />
      <FormSubmissionsToDateFilter />
    </FilteringContainer>
  );
};
