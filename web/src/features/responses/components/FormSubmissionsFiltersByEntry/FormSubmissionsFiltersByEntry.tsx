import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
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
import { useDataSource } from '@/common/data-source-store';
import { DataSources } from '@/common/types';
import { CoalitionMemberFilter } from '@/features/filtering/components/CoalitionMemberFilter';

export const FormSubmissionsFiltersByEntry: FC = () => {
  const dataSource = useDataSource();

  return (
    <FilteringContainer>
      <FormTypeFilter />
      <FormSubmissionsFormFilter />
      {dataSource === DataSources.Coalition ? <CoalitionMemberFilter /> : null}
      <FormSubmissionsFlaggedAnswersFilter />
      <FormSubmissionsFollowUpFilter />
      <FormSubmissionsQuestionsAnsweredFilter />
      <FormSubmissionsQuestionNotesFilter />
      <FormSubmissionsMediaFilesFilter />
      <MonitoringObserverTagsSelect isFilteringFormSubmissions />
      <PollingStationsFilters />
      <FormSubmissionsFromDateFilter />
      <FormSubmissionsToDateFilter />
    </FilteringContainer>
  );
};
