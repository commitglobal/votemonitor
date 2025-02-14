import { useDataSource } from '@/common/data-source-store';
import { DataSources } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { CoalitionMemberFilter } from '@/features/filtering/components/CoalitionMemberFilter';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsFollowUpFilter } from '@/features/filtering/components/FormSubmissionsFollowUpFilter';
import { FormSubmissionsFormFilter } from '@/features/filtering/components/FormSubmissionsFormFilter';
import { FormSubmissionsFromDateFilter } from '@/features/filtering/components/FormSubmissionsFromDateFilter';
import { FormSubmissionsQuestionsAnsweredFilter } from '@/features/filtering/components/FormSubmissionsQuestionsAnsweredFilter';
import { FormSubmissionsToDateFilter } from '@/features/filtering/components/FormSubmissionsToDateFilter';
import { FormTypeFilter } from '@/features/filtering/components/FormTypeFilter';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FC } from 'react';
import { FormSubmissionsFlaggedAnswersFilter } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsMediaFilesFilter } from '../../../filtering/components/FormSubmissionsMediaFilesFilter';
import { FormSubmissionsQuestionNotesFilter } from '../../../filtering/components/FormSubmissionsQuestionNotesFilter';

export const FormSubmissionsFiltersByForm: FC = () => {
  const dataSource = useDataSource();

  return (
    <FilteringContainer>
      <FormSubmissionsFormFilter />
      {dataSource === DataSources.Coalition ? <CoalitionMemberFilter /> : null}
      <FormTypeFilter />
      <FormSubmissionsFlaggedAnswersFilter />
      <FormSubmissionsFollowUpFilter />
      <FormSubmissionsQuestionsAnsweredFilter />
      <FormSubmissionsQuestionNotesFilter />
      <FormSubmissionsMediaFilesFilter />
      <MonitoringObserverTagsSelect isUsingAlternativeFilteringKey />
      <PollingStationsFilters />
      <FormSubmissionsFromDateFilter />
      <FormSubmissionsToDateFilter />
    </FilteringContainer>
  );
};
