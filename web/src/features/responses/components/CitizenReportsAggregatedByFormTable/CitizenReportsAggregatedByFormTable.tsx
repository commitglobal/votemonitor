import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getRouteApi } from '@tanstack/react-router';
import { useCallback } from 'react';
import { useCitizenReportsAggregatedByForm } from '../../hooks/citizen-reports';
import { citizenReportsAggregatedByFormColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

export function CitizenReportsAggregatedByFormTable(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const navigateToAggregatedForm = useCallback(
    (formId: string) => {
      navigate({ to: '/responses/citizen-reports/$formId/aggregated', params: { formId } });
    },
    [navigate]
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
