import { getRouteApi } from '@tanstack/react-router';
import { useCallback } from 'react';
import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useFormSubmissionsByForm } from '../../hooks/form-submissions-queries';
import { useByFormColumns } from '../../store/column-visibility';
import { formSubmissionsByFormColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

export function FormsTableByForm(): FunctionComponent {
  const columnsVisibility = useByFormColumns();
  const navigate = routeApi.useNavigate();

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
        useQuery={useFormSubmissionsByForm}
        onRowClick={navigateToAggregatedForm}
      />
    </CardContent>
  );
}
