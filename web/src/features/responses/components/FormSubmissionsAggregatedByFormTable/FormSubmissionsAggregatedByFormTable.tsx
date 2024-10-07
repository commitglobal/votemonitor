import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useFormSubmissionsByForm } from '../../hooks/form-submissions-queries';
import { FormSubmissionsSearchParams } from '../../models/search-params';
import { useFormSubmissionsByFormColumns } from '../../store/column-visibility';
import { formSubmissionsByFormColumnDefs } from '../../utils/column-defs';

const routeApi = getRouteApi('/responses/');

type FormSubmissionsByFormTableProps = {
  searchText: string;
};

export function FormSubmissionsAggregatedByFormTable({
  searchText,
}: FormSubmissionsByFormTableProps): FunctionComponent {
  const columnsVisibility = useFormSubmissionsByFormColumns();
  const navigate = routeApi.useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const navigateToAggregatedForm = useCallback(
    (formId: string) => {
      void navigate({ to: '/responses/$formId/aggregated', params: { formId } });
    },
    [navigate]
  );

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', searchText],
      ['hasFlaggedAnswers', debouncedSearch.hasFlaggedAnswers],
      ['hasNotes', debouncedSearch.hasNotes],
      ['hasAttachments', debouncedSearch.hasAttachments],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormSubmissionsSearchParams;
  }, [searchText, debouncedSearch]);

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsByFormColumnDefs}
        useQuery={(params) => useFormSubmissionsByForm(currentElectionRoundId, params)}
        onRowClick={navigateToAggregatedForm}
        queryParams={queryParams}
      />
    </CardContent>
  );
}
