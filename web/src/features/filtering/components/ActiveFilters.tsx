import { FilterBadge } from '@/components/ui/badge';
import { useNavigate } from '@tanstack/react-router';
import { FC, useCallback } from 'react';
import { FILTER_KEY, FILTER_LABEL } from '../filtering-enums';

interface ActiveFilterProps {
  filterId: string;
  value: string;
  isArray?: boolean;
}

type SearchParams = {
  [key: string]: any;
};

const HIDDEN_FILTERS = [FILTER_KEY.PageSize, FILTER_KEY.PageNumber, FILTER_KEY.ViewBy];
const FILTER_LABELS = new Map<string, string>([
  [FILTER_KEY.MonitoringObserverStatus, FILTER_LABEL.MonitoringObserverStatus],
  [FILTER_KEY.MonitoringObserverTags, FILTER_LABEL.MonitoringObserverTags],
  [FILTER_KEY.FormTypeFilter, FILTER_LABEL.FormTypeFilter],
  [FILTER_KEY.HasFlaggedAnswers, FILTER_LABEL.HasFlaggedAnswers],
  [FILTER_KEY.FollowUpStatus, FILTER_LABEL.FollowUpStatus],
  [FILTER_KEY.HasNotes, FILTER_LABEL.HasNotes],
  [FILTER_KEY.MediaFiles, FILTER_LABEL.MediaFiles],
  [FILTER_KEY.QuestionsAnswered, FILTER_LABEL.QuestionsAnswered],
  [FILTER_KEY.LocationL1, FILTER_LABEL.LocationL1],
  [FILTER_KEY.LocationL2, FILTER_LABEL.LocationL2],
  [FILTER_KEY.LocationL3, FILTER_LABEL.LocationL3],
  [FILTER_KEY.LocationL4, FILTER_LABEL.LocationL4],
  [FILTER_KEY.LocationL5, FILTER_LABEL.LocationL5],
  [FILTER_KEY.FormSubmissionsMonitoringObserverTags, FILTER_LABEL.FormSubmissionsMonitoringObserverTags],
  [FILTER_KEY.PollingStationNumber, FILTER_LABEL.PollingStationNumber],
]);

const ActiveFilter: FC<ActiveFilterProps> = ({ filterId, value, isArray }) => {
  const label = FILTER_LABELS.get(filterId) ?? filterId;

  const navigate = useNavigate();
  const onClearFilter = useCallback(
    (filter: string, value?: string) => () => {
      if (!isArray) {
        return navigate({ search: (prev) => ({ ...prev, [filter]: undefined }) });
      }
      return navigate({
        search: (prev: SearchParams) => ({
          ...prev,
          [filter]: prev[filter]?.filter((item: string) => item !== value), // Remove the value from the array
        }),
      });
    },
    [navigate]
  );
  return <FilterBadge label={`${label}: ${value}`} onClear={onClearFilter(filterId, value)} />;
};

interface ActiveFiltersProps {
  queryParams: any;
}

export const ActiveFilters: FC<ActiveFiltersProps> = ({ queryParams }) => {
  return (
    <div className='flex flex-wrap gap-2 col-span-full'>
      {Object.keys(queryParams).map((filterId) => {
        let key = '';
        const value = queryParams[filterId];
        const isArray = Array.isArray(value);

        if (HIDDEN_FILTERS.includes(filterId)) return;

        if (!isArray) {
          key = `active-filter-${filterId}`;
          return <ActiveFilter key={key} filterId={filterId} value={value} />;
        }
        return value.map((item) => {
          key = `active-filter-${filterId}-${item}`;

          return <ActiveFilter key={key} filterId={filterId} value={item as string} isArray />;
        });
      })}
    </div>
  );
};
