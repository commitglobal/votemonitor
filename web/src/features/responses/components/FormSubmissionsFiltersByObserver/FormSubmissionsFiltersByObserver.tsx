import { useDataSource } from '@/common/data-source-store';
import { DataSources, type FunctionComponent } from '@/common/types';
import { CoalitionMemberFilter } from '@/features/filtering/components/CoalitionMemberFilter';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FormSubmissionsFlaggedAnswersFilter } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsFollowUpFilter } from '../../../filtering/components/FormSubmissionsFollowUpFilter';

export function FormSubmissionsFiltersByObserver(): FunctionComponent {
  const dataSource = useDataSource();
  
  return (
    <FilteringContainer>
      <FormSubmissionsFollowUpFilter />
      {dataSource === DataSources.Coalition ? <CoalitionMemberFilter /> : null}
      <MonitoringObserverTagsSelect isFilteringFormSubmissions />
      <FormSubmissionsFlaggedAnswersFilter />
    </FilteringContainer>
  );
}
