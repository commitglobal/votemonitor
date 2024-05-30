import type { FunctionComponent } from '@/common/types';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { CardContent } from '@/components/ui/card';
import { useFormSubmissionsByEntry } from '@/features/responses/hooks/form-submissions-queries';
import { formSubmissionsForObserverColumnDefs } from '@/features/responses/utils/column-defs';
import { getRouteApi } from '@tanstack/react-router';
import type { VisibilityState } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import type { MonitoringObserverDetailsRouteSearch } from '../../models/monitoring-observer';
import { formSubmissionsByObserverColumns } from '@/features/responses/utils/column-visibility-options';

const routeApi = getRouteApi('/monitoring-observers/view/$monitoringObserverId/$tab');

type FormsTableByEntryProps = {
  columnsVisibility: VisibilityState;
  searchText: string;
};

export function MonitoringObserverFormsTable({
  columnsVisibility,
  searchText,
}: FormsTableByEntryProps): FunctionComponent {
  const navigate = routeApi.useNavigate();
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
      ['followUpStatus', debouncedSearch.followUpStatus],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as MonitoringObserverDetailsRouteSearch;
  }, [searchText, debouncedSearch, monitoringObserverId]);

  const navigateToFormSubmission = useCallback(
    (submissionId: string) => {
      void navigate({ to: '/responses/$submissionId', params: { submissionId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsForObserverColumnDefs}
        useQuery={useFormSubmissionsByEntry}
        queryParams={queryParams}
        onRowClick={navigateToFormSubmission}
      />
    </CardContent>
  );
}
