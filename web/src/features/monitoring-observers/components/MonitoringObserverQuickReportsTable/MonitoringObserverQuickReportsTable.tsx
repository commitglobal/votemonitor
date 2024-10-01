import type { FunctionComponent } from '@/common/types';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { CardContent } from '@/components/ui/card';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useQuickReports } from '@/features/responses/hooks/quick-reports';
import { observerQuickReportsColumnDefs } from '@/features/responses/utils/column-defs';
import { getRouteApi } from '@tanstack/react-router';
import type { VisibilityState } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import type { MonitoringObserverDetailsRouteSearch } from '../../models/monitoring-observer';

const routeApi = getRouteApi('/monitoring-observers/view/$monitoringObserverId/$tab');

type QuickReportsTableByEntryProps = {
  columnsVisibility: VisibilityState;
  searchText: string;
};

export function MonitoringObserverQuickReportsTable({
  columnsVisibility,
  searchText,
}: QuickReportsTableByEntryProps): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const { monitoringObserverId } = routeApi.useParams();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const queryParams = useMemo(() => {
    const params = [
      ['formCodeFilter', searchText],
      ['formTypeFilter', debouncedSearch.formTypeFilter],
      ['hasFlaggedAnswers', debouncedSearch.hasFlaggedAnswers],
      ['quickReportLocationType', debouncedSearch.quickReportLocationType],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['monitoringObserverId', monitoringObserverId],
      ['followUpStatus', debouncedSearch.quickReportFollowUpStatus],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as MonitoringObserverDetailsRouteSearch;
  }, [searchText, debouncedSearch, monitoringObserverId]);

  const navigateToQuickReport = useCallback(
    (quickReportId: string) => {
      void navigate({ to: '/responses/quick-reports/$quickReportId', params: { quickReportId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={observerQuickReportsColumnDefs}
        useQuery={(params) => useQuickReports(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToQuickReport}
      />
    </CardContent>
  );
}
