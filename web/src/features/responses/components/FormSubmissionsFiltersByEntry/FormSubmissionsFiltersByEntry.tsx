import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsCompletionFilter } from '@/features/filtering/components/FormSubmissionsCompletionFilter';
import { FormTypeFilter } from '@/features/filtering/components/FormTypeFilter';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FC } from 'react';
import { FormSubmissionsFlaggedAnswersSelect } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsFollowUpSelect } from '../../../filtering/components/FormSubmissionsFollowUpFilter';
import { FormSubmissionsFormSelect } from '../../../filtering/components/FormSubmissionsFormFilter';
import { FormSubmissionsFromDateFilter } from '../../../filtering/components/FormSubmissionsFromDateFilter';
import { FormSubmissionsMediaFilesSelect } from '../../../filtering/components/FormSubmissionsMediaFilesFilter';
import { FormSubmissionsQuestionNotesSelect } from '../../../filtering/components/FormSubmissionsQuestionNotesFilter';
import { FormSubmissionsQuestionsAnsweredFilter } from '../../../filtering/components/FormSubmissionsQuestionsAnsweredFilter';
import { FormSubmissionsToDateFilter } from '../../../filtering/components/FormSubmissionsToDateFilter';

export const FormSubmissionsFiltersByEntry: FC = () => {
  return (
    <FilteringContainer>
      <FormTypeFilter />
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
