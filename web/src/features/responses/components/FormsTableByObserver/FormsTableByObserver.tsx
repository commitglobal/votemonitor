import type { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useFormSubmissionsByObserver } from '../../hooks/form-submissions-queries';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { useFormSubmissionsByObserverColumns } from '../../store/column-visibility';
import { formSubmissionsByObserverColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

type FormsTableByObserverProps = {
  searchText: string;
};

export function FormsTableByObserver({ searchText }: FormsTableByObserverProps): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const columnsVisibility = useFormSubmissionsByObserverColumns();

  const queryParams = useMemo(() => {
    const params = [
      ['followUpStatus', debouncedSearch.followUpStatus],
      ['searchText', searchText],
      ['tagsFilter', debouncedSearch.tagsFilter],
      ['hasFlaggedAnswers', debouncedSearch.hasFlaggedAnswers],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormSubmissionsSearchParams;
  }, [searchText, debouncedSearch]);

  const navigateToMonitoringObserver = useCallback(
    (monitoringObserverId: string) => {
      void navigate({
        to: '/monitoring-observers/view/$monitoringObserverId/$tab',
        params: { monitoringObserverId, tab: 'details' },
      });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsByObserverColumnDefs}
        useQuery={(params) => useFormSubmissionsByObserver(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToMonitoringObserver}
      />
    </CardContent>
  );
}
