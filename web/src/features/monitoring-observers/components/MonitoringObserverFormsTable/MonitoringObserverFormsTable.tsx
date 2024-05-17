import { getRouteApi } from '@tanstack/react-router';
import type { VisibilityState } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { useMemo } from 'react';
import type { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useFormSubmissionsByEntry } from '@/features/responses/hooks/form-submissions-queries';
import { formSubmissionsByEntryColumnDefs } from '@/features/responses/utils/column-defs';
import type { MonitoringObserverDetailsRouteSearch } from '../../models/MonitoringObserver';

const routeApi = getRouteApi('/monitoring-observers/$monitoringObserverId');

type FormsTableByEntryProps = {
  columnsVisibility: VisibilityState;
  searchText: string;
};

export function MonitoringObserverFormsTable({
  columnsVisibility,
  searchText,
}: FormsTableByEntryProps): FunctionComponent {
  const { monitoringObserverId } = routeApi.useParams();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const queryParams = useMemo(() => {
    const params = [
      ['formCodeFilter', searchText],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['formTypeFilter', debouncedSearch.formTypeFilter],
      ['hasFlaggedAnswers', debouncedSearch.hasFlaggedAnswers],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['monitoringObserverId', monitoringObserverId],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as MonitoringObserverDetailsRouteSearch;
  }, [searchText, debouncedSearch, monitoringObserverId]);

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsByEntryColumnDefs}
        useQuery={useFormSubmissionsByEntry}
        queryParams={queryParams}
      />
    </CardContent>
  );
}
