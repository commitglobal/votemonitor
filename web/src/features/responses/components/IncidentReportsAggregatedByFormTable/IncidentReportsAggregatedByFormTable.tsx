import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { Route } from '@/routes/responses';
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useIncidentReportsByForm } from '../../hooks/incident-reports-queries';
import { useIncidentReportsByFormColumns } from '../../store/column-visibility';
import { incidentReportsByFormColumnDefs } from '../../utils/column-defs';
import { IncidentReportsSearchParams } from '../../models/search-params';

export function IncidentReportsAggregatedByFormTable(): FunctionComponent {
  const columnsVisibility = useIncidentReportsByFormColumns();
  const navigate = useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const queryParams = useMemo(() => {
    const params = [
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
  }, [debouncedSearch]);
  const navigateToAggregatedForm = useCallback(
    (formId: string) => {
      void navigate({ to: '/responses/incident-reports/$formId/aggregated', params: { formId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={incidentReportsByFormColumnDefs}
        useQuery={(params) => useIncidentReportsByForm(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToAggregatedForm}
      />
    </CardContent>
  );
}
