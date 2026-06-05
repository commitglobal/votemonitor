import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useSetPrevSearch } from '@/common/prev-search-store';
import { getRouteApi, useSearch } from '@tanstack/react-router';
import { useCallback } from 'react';
import { useCitizenReportsAggregatedByForm } from '../../hooks/citizen-reports';
import { citizenReportsAggregatedByFormColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

export function CitizenReportsAggregatedByFormTable(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const search = routeApi.useSearch();
  const fullSearch = useSearch({ strict: false });
  const setPrevSearch = useSetPrevSearch();

  const navigateToAggregatedForm = useCallback(
    (formId: string) => {
      setPrevSearch(fullSearch);
      navigate({ to: '/responses/citizen-reports/$formId/aggregated', params: { formId } });
    },
    [navigate, setPrevSearch, fullSearch]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columns={citizenReportsAggregatedByFormColumnDefs}
        useQuery={(params) => useCitizenReportsAggregatedByForm(currentElectionRoundId, params)}
        onRowClick={navigateToAggregatedForm}
      />
    </CardContent>
  );
}
