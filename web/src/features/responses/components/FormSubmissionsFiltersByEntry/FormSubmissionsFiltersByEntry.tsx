import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FC } from 'react';
import { FormSubmissionsFlaggedAnswersSelect } from '../../filtering/FormSubmissionsFlaggedAnswersSelect';
import { FormSubmissionsFollowUpSelect } from '../../filtering/FormSubmissionsFollowUpSelect';
import { FormSubmissionsMediaFilesSelect } from '../../filtering/FormSubmissionsMediaFilesSelect';
import { FormSubmissionsQuestionNotesSelect } from '../../filtering/FormSubmissionsQuestionNotesSelect';
import { FormSubmissionsQuestionsAnsweredSelect } from '../../filtering/FormSubmissionsQuestionsAnsweredSelect';
import { FormSubmissionsTypeSelect } from '../../filtering/FormSubmissionsTypeSelect';
import { FormSubmissionsFormSelect } from '../../filtering/FormSubmissionsFormSelect';
import { FormSubmissionsFromDateFilter } from '../../filtering/FormSubmissionsFromDateFilter';
import { FormSubmissionsToDateFilter } from '../../filtering/FormSubmissionsToDateFilter';

export const FormSubmissionsFiltersByEntry: FC = () => {
  return (
    <FilteringContainer>
      <FormSubmissionsTypeSelect />
      <FormSubmissionsFormSelect />
      <FormSubmissionsFlaggedAnswersSelect />
      <FormSubmissionsFollowUpSelect />
      <FormSubmissionsQuestionsAnsweredSelect />
      <FormSubmissionsQuestionNotesSelect />
      <FormSubmissionsMediaFilesSelect />
      <MonitoringObserverTagsSelect isFilteringFormSubmissions />
      <PollingStationsFilters />
      <FormSubmissionsFromDateFilter />
      <FormSubmissionsToDateFilter />
    </FilteringContainer>
  );
};
