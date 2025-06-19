import { DataSources, FormSubmissionFollowUpStatus, FunctionComponent } from '@/common/types';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { CardContent } from '@/components/ui/card';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getValueOrDefault, toBoolean } from '@/lib/utils';
import { Route } from '@/routes/(app)/responses';
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useFormSubmissionsByObserver } from '../../hooks/form-submissions-queries';
import { useFormSubmissionsByObserverColumns } from '../../store/column-visibility';
import { formSubmissionsByObserverColumnDefs } from '../../utils/column-defs';

type FormSubmissionsByObserverTableProps = {
  searchText: string;
};

export interface FormSubmissionsByObserverFilterRequest {
  followUpStatus: FormSubmissionFollowUpStatus | undefined;
  searchText: string | undefined;
  tagsFilter: string[] | undefined;
  hasFlaggedAnswers: boolean | undefined;
  dataSource: DataSources;
  coalitionMemberId: string | undefined;
}
export function FormSubmissionsByObserverTable({ searchText }: FormSubmissionsByObserverTableProps): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const columnsVisibility = useFormSubmissionsByObserverColumns();

  const queryParams = useMemo(() => {
    const params: FormSubmissionsByObserverFilterRequest = {
      dataSource: getValueOrDefault(search.dataSource, DataSources.Ngo),
      followUpStatus: debouncedSearch.followUpStatus,
      searchText: searchText,
      tagsFilter: debouncedSearch.tagsFilter,
      hasFlaggedAnswers: toBoolean(debouncedSearch.hasFlaggedAnswers),
      coalitionMemberId: debouncedSearch.coalitionMemberId,
    };

    return params;
  }, [searchText, debouncedSearch]);

  const navigateToMonitoringObserver = useCallback(
    (monitoringObserverId: string) => {
      navigate({
        to: '/monitoring-observers/view/$monitoringObserverId/$tab',
        params: { monitoringObserverId, tab: 'details' },
      });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsByObserverColumnDefs}
        useQuery={(params) => useFormSubmissionsByObserver(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToMonitoringObserver}
      />
    </CardContent>
  );
}
