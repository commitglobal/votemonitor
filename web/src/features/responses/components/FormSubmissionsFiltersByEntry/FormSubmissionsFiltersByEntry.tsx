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

export const FormSubmissionsFiltersByEntry: FC = () => {
  return (
    <FilteringContainer>
      <FormSubmissionsTypeSelect />
      <FormSubmissionsFlaggedAnswersSelect />
      <FormSubmissionsFollowUpSelect />
      <FormSubmissionsQuestionsAnsweredSelect />
      <FormSubmissionsQuestionNotesSelect />
      <FormSubmissionsMediaFilesSelect />
      <MonitoringObserverTagsSelect isFilteringFormSubmissions />
      <PollingStationsFilters />
    </FilteringContainer>
  );
};
