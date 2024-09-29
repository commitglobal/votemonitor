import type { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { Route } from '@/routes/responses';
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useIssueReportsByEntry } from '../../hooks/issue-reports-queries';
import type { IssueReportsSearchParams } from '../../models/search-params';
import { useIssueReportsByEntryColumns } from '../../store/column-visibility';
import { issueReportsByEntryColumnDefs } from '../../utils/column-defs';

type FormsTableByEntryProps = {
  searchText: string;
};

export function IssueReportsByEntryTable({ searchText }: FormsTableByEntryProps): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);

  const columnsVisibility = useIssueReportsByEntryColumns();

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', searchText],
      ['hasFlaggedAnswers', debouncedSearch.hasFlaggedAnswers],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['issueReportFollowUpStatus', debouncedSearch.issueReportFollowUpStatus],
      ['issueReportLocationType', debouncedSearch.issueReportLocationType],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as IssueReportsSearchParams;
  }, [searchText, debouncedSearch]);

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
        columns={issueReportsByEntryColumnDefs}
        useQuery={(params) => useIssueReportsByEntry(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToIssueReport}
      />
    </CardContent>
  );
}
