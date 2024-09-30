import { type FunctionComponent } from '@/common/types';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';
import { FormSubmissionsFlaggedAnswersSelect } from '../../filtering/FormSubmissionsFlaggedAnswersSelect';
import { FormSubmissionsFollowUpSelect } from '../../filtering/FormSubmissionsFollowUpSelect';

export function FormsFiltersByObserver(): FunctionComponent {
  return (
    <FilteringContainer>
      <FormSubmissionsFollowUpSelect />
      <MonitoringObserverTagsSelect isFilteringFormSubmissions />
      <FormSubmissionsFlaggedAnswersSelect />
    </FilteringContainer>
  );
}
