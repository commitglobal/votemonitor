import { getRouteApi } from '@tanstack/react-router';
import { useCallback } from 'react';
import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useFormSubmissionsByForm } from '../../hooks/form-submissions-queries';
import { useByFormColumns } from '../../store/column-visibility';
import { formSubmissionsByFormColumnDefs } from '../../utils/column-defs';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';

const routeApi = getRouteApi('/responses/');

export function FormsTableByForm(): FunctionComponent {
  const columnsVisibility = useByFormColumns();
  const navigate = routeApi.useNavigate();
    const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);

  const navigateToAggregatedForm = useCallback(
    (formId: string) => {
      void navigate({ to: '/responses/$formId/aggregated', params: { formId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsByFormColumnDefs}
        useQuery={(params) => useFormSubmissionsByForm(currentElectionRoundId, params)}
        onRowClick={navigateToAggregatedForm}
      />
    </CardContent>
  );
}
