import { FunctionComponent } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useCitizenReportsAggregatedByForm } from '../../hooks/citizen-reports';
import { FormSubmissionsSearchParams } from '../../models/search-params';
import { citizenReportsAggregatedByFormColumnDefs } from '../../utils/column-defs';
import { useCitizenReportsColumnsVisibility } from '../../store/column-visibility';

const routeApi = getRouteApi('/responses/');

export function CitizenReportsAggregatedByFormTable(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const columnsVisibility = useCitizenReportsColumnsVisibility();

  const queryParams = useMemo(() => {
    const params = [
      ['hasFlaggedAnswers', debouncedSearch.hasFlaggedAnswers],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['followUpStatus', debouncedSearch.followUpStatus],
      ['questionsAnswered', debouncedSearch.questionsAnswered],
      ['hasNotes', debouncedSearch.hasNotes],
      ['hasAttachments', debouncedSearch.hasAttachments],
      ['formId', debouncedSearch.formId],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormSubmissionsSearchParams;
  }, [debouncedSearch]);

  const navigateToAggregatedForm = useCallback(
    (formId: string) => {
      void navigate({ to: '/responses/citizen-reports/$formId/aggregated', params: { formId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columns={citizenReportsAggregatedByFormColumnDefs}
        useQuery={(params) => useCitizenReportsAggregatedByForm(currentElectionRoundId, params)}
        onRowClick={navigateToAggregatedForm}
        queryParams={queryParams}
        columnVisibility={columnsVisibility}
      />
    </CardContent>
  );
}
