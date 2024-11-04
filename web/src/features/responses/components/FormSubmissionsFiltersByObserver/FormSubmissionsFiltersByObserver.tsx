import { type FunctionComponent } from '@/common/types';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FormSubmissionsFlaggedAnswersFilter } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsFollowUpFilter } from '../../../filtering/components/FormSubmissionsFollowUpFilter';

export function FormSubmissionsFiltersByObserver(): FunctionComponent {
  return (
    <FilteringContainer>
      <FormSubmissionsFollowUpFilter />
      <MonitoringObserverTagsSelect isFilteringFormSubmissions />
      <FormSubmissionsFlaggedAnswersFilter />
    </FilteringContainer>
  );
}
