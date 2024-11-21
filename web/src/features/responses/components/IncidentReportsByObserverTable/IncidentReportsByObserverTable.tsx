import type { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useIncidentReportsByObserver } from '../../hooks/incident-reports-queries';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { useIncidentReportsByObserverColumns } from '../../store/column-visibility';
import { incidentReportsByObserverColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

type IncidentReportsByObserverTableProps = {
  searchText: string;
};

export function IncidentReportsByObserverTable({ searchText }: IncidentReportsByObserverTableProps): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const columnsVisibility = useIncidentReportsByObserverColumns();

  const queryParams = useMemo(() => {
    const params = [
      ['followUpStatus', debouncedSearch.followUpStatus],
      ['searchText', searchText],
      ['tagsFilter', debouncedSearch.tags],
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
        columns={incidentReportsByObserverColumnDefs}
        useQuery={(params) => useIncidentReportsByObserver(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToMonitoringObserver}
      />
    </CardContent>
  );
}
