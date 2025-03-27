import type { FunctionComponent } from '@/common/types';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { CardContent } from '@/components/ui/card';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useIncidentReportsByEntry } from '@/features/responses/hooks/incident-reports-queries';
import { observerIncidentReportsColumnDefs } from '@/features/responses/utils/column-defs';
import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';
import { useNavigate } from '@tanstack/react-router';
import type { VisibilityState } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import type { MonitoringObserverDetailsRouteSearch } from '../../models/monitoring-observer';

type IncidentReportsTableByEntryProps = {
  columnsVisibility: VisibilityState;
  searchText: string;
};

export function MonitoringObserverIncidentReportsTable({
  columnsVisibility,
  searchText,
}: IncidentReportsTableByEntryProps): FunctionComponent {
  const navigate = useNavigate();
  const { monitoringObserverId } = Route.useParams();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const queryParams = useMemo(() => {
    const params = [
      ['formCodeFilter', searchText],
      ['formTypeFilter', debouncedSearch.formTypeFilter],
      ['hasFlaggedAnswers', debouncedSearch.hasFlaggedAnswers],
      ['locationType', debouncedSearch.incidentReportLocationType],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['monitoringObserverId', monitoringObserverId],
      ['followUpStatus', debouncedSearch.incidentReportFollowUpStatus],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as MonitoringObserverDetailsRouteSearch;
  }, [searchText, debouncedSearch, monitoringObserverId]);

  const navigateToIncidentReport = useCallback(
    (incidentReportId: string) => {
      navigate({ to: '/responses/incident-reports/$incidentReportId', params: { incidentReportId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={observerIncidentReportsColumnDefs}
        useQuery={(params) => useIncidentReportsByEntry(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToIncidentReport}
      />
    </CardContent>
  );
}
