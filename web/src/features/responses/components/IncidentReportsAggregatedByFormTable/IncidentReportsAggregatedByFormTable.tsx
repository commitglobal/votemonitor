import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getRouteApi } from '@tanstack/react-router';
import { useCallback } from 'react';
import { useIncidentReportsByForm } from '../../hooks/incident-reports-queries';
import { useIncidentReportsByFormColumns } from '../../store/column-visibility';
import { incidentReportsByFormColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

export function IncidentReportsAggregatedByFormTable(): FunctionComponent {
  const columnsVisibility = useIncidentReportsByFormColumns();
  const navigate = routeApi.useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

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
        onRowClick={navigateToAggregatedForm}
      />
    </CardContent>
  );
}
