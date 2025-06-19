import type { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { Route } from '@/routes/(app)/responses';
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useIncidentReportsByEntry } from '../../hooks/incident-reports-queries';
import type { IncidentReportsSearchParams } from '../../models/search-params';
import { useIncidentReportsByEntryColumns } from '../../store/column-visibility';
import { incidentReportsByEntryColumnDefs } from '../../utils/column-defs';

type FormsTableByEntryProps = {
  searchText: string;
};

export function IncidentReportsByEntryTable({ searchText }: FormsTableByEntryProps): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const columnsVisibility = useIncidentReportsByEntryColumns();

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
      ['followUpStatus', debouncedSearch.incidentReportFollowUpStatus],
      ['locationType', debouncedSearch.incidentReportLocationType],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as IncidentReportsSearchParams;
  }, [searchText, debouncedSearch]);

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
        columns={incidentReportsByEntryColumnDefs}
        useQuery={(params) => useIncidentReportsByEntry(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToIncidentReport}
      />
    </CardContent>
  );
}
