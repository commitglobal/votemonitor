import { type FunctionComponent } from '@/common/types';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FormSubmissionsFlaggedAnswersSelect } from '../../../filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsFollowUpSelect } from '../../../filtering/components/FormSubmissionsFollowUpFilter';

export function FormSubmissionsFiltersByObserver(): FunctionComponent {
  return (
    <FilteringContainer>
      <FormSubmissionsFollowUpSelect />
      <MonitoringObserverTagsSelect isFilteringFormSubmissions />
      <FormSubmissionsFlaggedAnswersSelect />
    </FilteringContainer>
  );
}
