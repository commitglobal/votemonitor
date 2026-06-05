import type { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useSetPrevSearch } from '@/common/prev-search-store';
import { Route } from '@/routes/responses';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useCitizenReports } from '../../hooks/citizen-reports';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { citizenReportsByEntryColumnDefs } from '../../utils/column-defs';

type CitizenReportsByEntryTableProps = {
};

export function CitizenReportsByEntryTable(props: CitizenReportsByEntryTableProps): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const fullSearch = useSearch({ strict: false });
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const setPrevSearch = useSetPrevSearch();

  const queryParams = useMemo(() => {
    const params = [
      ['followUpStatus', debouncedSearch.citizenReportFollowUpStatus],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormSubmissionsSearchParams;
  }, [debouncedSearch]);

  const navigateToCitizenReport = useCallback(
    (citizenReportId: string) => {
      setPrevSearch(fullSearch);
      navigate({ to: '/responses/citizen-reports/$citizenReportId', params: { citizenReportId } });
    },
    [navigate, setPrevSearch, fullSearch]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columns={citizenReportsByEntryColumnDefs}
        useQuery={(params) => useCitizenReports(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToCitizenReport}
      />
    </CardContent>
  );
}
