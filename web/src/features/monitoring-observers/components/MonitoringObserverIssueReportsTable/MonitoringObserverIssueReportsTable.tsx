import type { FunctionComponent } from '@/common/types';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { CardContent } from '@/components/ui/card';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useIssueReportsByEntry } from '@/features/responses/hooks/issue-reports-queries';
import { observerIssueReportsColumnDefs } from '@/features/responses/utils/column-defs';
import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';
import { useNavigate } from '@tanstack/react-router';
import type { VisibilityState } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import type { MonitoringObserverDetailsRouteSearch } from '../../models/monitoring-observer';

type IssueReportsTableByEntryProps = {
  columnsVisibility: VisibilityState;
  searchText: string;
};

export function MonitoringObserverIssueReportsTable({
  columnsVisibility,
  searchText,
}: IssueReportsTableByEntryProps): FunctionComponent {
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
      ['locationType', debouncedSearch.issueReportLocationType],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['monitoringObserverId', monitoringObserverId],
      ['followUpStatus', debouncedSearch.issueReportFollowUpStatus],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as MonitoringObserverDetailsRouteSearch;
  }, [searchText, debouncedSearch, monitoringObserverId]);

  const navigateToIssueReport = useCallback(
    (issueReportId: string) => {
      void navigate({ to: '/responses/issue-reports/$issueReportId', params: { issueReportId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={observerIssueReportsColumnDefs}
        useQuery={(params) => useIssueReportsByEntry(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToIssueReport}
      />
    </CardContent>
  );
}
