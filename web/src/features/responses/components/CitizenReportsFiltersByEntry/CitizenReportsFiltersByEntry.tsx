import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsCompletionFilter } from '@/features/filtering/components/FormSubmissionsCompletionFilter';
import { FormTypeFilter } from '@/features/filtering/components/FormTypeFilter';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FC } from 'react';
import { FormSubmissionsFlaggedAnswersFilter } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsFollowUpFilter } from '../../../filtering/components/FormSubmissionsFollowUpFilter';
import { FormSubmissionsFormFilter } from '../../../filtering/components/FormSubmissionsFormFilter';
import { FormSubmissionsFromDateFilter } from '../../../filtering/components/FormSubmissionsFromDateFilter';
import { FormSubmissionsMediaFilesFilter } from '../../../filtering/components/FormSubmissionsMediaFilesFilter';
import { FormSubmissionsQuestionNotesFilter } from '../../../filtering/components/FormSubmissionsQuestionNotesFilter';
import { FormSubmissionsQuestionsAnsweredFilter } from '../../../filtering/components/FormSubmissionsQuestionsAnsweredFilter';
import { FormSubmissionsToDateFilter } from '../../../filtering/components/FormSubmissionsToDateFilter';

export const CitizenReportsFiltersByEntry: FC = () => {
  return (
    <FilteringContainer>
      <FormTypeFilter />
      <FormSubmissionsFormFilter />
      <FormSubmissionsFlaggedAnswersFilter />
      <FormSubmissionsFollowUpFilter />
      <FormSubmissionsQuestionsAnsweredFilter />
      <FormSubmissionsQuestionNotesFilter />
      <FormSubmissionsMediaFilesFilter />
      <PollingStationsFilters />
      <FormSubmissionsFromDateFilter />
      <FormSubmissionsToDateFilter />
    </FilteringContainer>
  );
};
