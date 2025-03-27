import { DataSources, type FunctionComponent } from '@/common/types';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { CardContent } from '@/components/ui/card';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { QuickReportFilterRequest } from '@/features/responses/components/QuickReportsTab/QuickReportsTab';
import { useQuickReports } from '@/features/responses/hooks/quick-reports';
import { observerQuickReportsColumnDefs } from '@/features/responses/utils/column-defs';
import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';
import { useNavigate } from '@tanstack/react-router';
import type { VisibilityState } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';

type QuickReportsTableByEntryProps = {
  columnsVisibility: VisibilityState;
  searchText: string;
};

export function MonitoringObserverQuickReportsTable({
  columnsVisibility,
  searchText,
}: QuickReportsTableByEntryProps): FunctionComponent {
  const navigate = useNavigate();
  const { monitoringObserverId } = Route.useParams();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const queryParams = useMemo(() => {
    const params: QuickReportFilterRequest = {
      quickReportLocationType: debouncedSearch.quickReportLocationType,
      level1Filter: debouncedSearch.level1Filter,
      level2Filter: debouncedSearch.level2Filter,
      level3Filter: debouncedSearch.level3Filter,
      level4Filter: debouncedSearch.level4Filter,
      level5Filter: debouncedSearch.level5Filter,
      pollingStationNumberFilter: debouncedSearch.pollingStationNumberFilter,
      quickReportFollowUpStatus: debouncedSearch.quickReportFollowUpStatus,
      incidentCategory: debouncedSearch.incidentCategory,
      dataSource: DataSources.Ngo,
      monitoringObserverId: debouncedSearch.monitoringObserverId,
      coalitionMemberId: undefined,
      searchText: searchText
    };

    return params;
  }, [searchText, debouncedSearch, monitoringObserverId]);

  const navigateToQuickReport = useCallback(
    (quickReportId: string) => {
      navigate({ to: '/responses/quick-reports/$quickReportId', params: { quickReportId } });
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
