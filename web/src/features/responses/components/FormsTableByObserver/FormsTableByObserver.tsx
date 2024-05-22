import { getRouteApi } from '@tanstack/react-router';
import type { VisibilityState } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import type { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useFormSubmissionsByObserver } from '../../hooks/form-submissions-queries';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { formSubmissionsByObserverColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

type FormsTableByObserverProps = {
  columnsVisibility: VisibilityState;
  searchText: string;
};

export function FormsTableByObserver({ columnsVisibility, searchText }: FormsTableByObserverProps): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', searchText],
      ['tagsFilter', debouncedSearch.tagsFilter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormSubmissionsSearchParams;
  }, [searchText, debouncedSearch]);

  const navigateToMonitoringObserver = useCallback(
    (monitoringObserverId: string) => {
      void navigate({ to: '/monitoring-observers/view/$monitoringObserverId/$tab', params: { monitoringObserverId, tab: 'details' } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsByObserverColumnDefs}
        useQuery={useFormSubmissionsByObserver}
        queryParams={queryParams}
        onRowClick={navigateToMonitoringObserver}
      />
    </CardContent>
  );
}
