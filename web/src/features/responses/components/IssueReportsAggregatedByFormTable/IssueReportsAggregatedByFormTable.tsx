import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getRouteApi } from '@tanstack/react-router';
import { useCallback } from 'react';
import { useIssueReportsByForm } from '../../hooks/issue-reports-queries';
import { useIssueReportsByFormColumns } from '../../store/column-visibility';
import { issueReportsByFormColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

export function IssueReportsAggregatedByFormTable(): FunctionComponent {
  const columnsVisibility = useIssueReportsByFormColumns();
  const navigate = routeApi.useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const navigateToAggregatedForm = useCallback(
    (formId: string) => {
      void navigate({ to: '/responses/issue-reports/$formId/aggregated', params: { formId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={issueReportsByFormColumnDefs}
        useQuery={(params) => useIssueReportsByForm(currentElectionRoundId, params)}
        onRowClick={navigateToAggregatedForm}
      />
    </CardContent>
  );
}
