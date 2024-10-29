import { type FunctionComponent } from '@/common/types';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsFollowUpFilter } from '@/features/filtering/components/FormSubmissionsFollowUpFilter';
import { MonitoringObserverTagsSelect } from '@/features/monitoring-observers/filtering/MonitoringObserverTagsSelect';

export function IncidentReportsFiltersByObserver(): FunctionComponent {
  return (
    <>
      <FilteringContainer>
        <FormSubmissionsFollowUpFilter />
        <MonitoringObserverTagsSelect />
      </FilteringContainer>
    </>
  );
}
