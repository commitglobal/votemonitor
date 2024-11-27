import { DataSources, FunctionComponent } from '@/common/types';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { CardContent } from '@/components/ui/card';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getValueOrDefault, toBoolean } from '@/lib/utils';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useFormSubmissionsByForm } from '../../hooks/form-submissions-queries';
import { useFormSubmissionsByFormColumns } from '../../store/column-visibility';
import { formSubmissionsByFormColumnDefs } from '../../utils/column-defs';
import { FormSubmissionsSearchRequest } from '../FormSubmissionsByEntryTable/FormSubmissionsByEntryTable';

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
      void navigate({
        to: '/responses/form-submissions/$formId/aggregated',
        params: { formId },
        search: {
          hasFlaggedAnswers: search.hasFlaggedAnswers,
          level1Filter: search.level1Filter,
          level2Filter: search.level2Filter,
          level3Filter: search.level3Filter,
          level4Filter: search.level4Filter,
          level5Filter: search.level5Filter,
          pollingStationNumberFilter: search.pollingStationNumberFilter,
          followUpStatus: search.followUpStatus,
          questionsAnswered: search.questionsAnswered,
          hasNotes: search.hasNotes,
          hasAttachments: search.hasAttachments,
          tagsFilter: search.tagsFilter,
          submissionsFromDate: search.submissionsFromDate,
          submissionsToDate: search.submissionsToDate,
          dataSource: getValueOrDefault(search.dataSource, DataSources.Ngo),
          coalitionMemberId: search.coalitionMemberId
        },
      });
    },
    [navigate, search]
  );

  const queryParams = useMemo(() => {
    const params: FormSubmissionsSearchRequest = {
      dataSource: getValueOrDefault(search.dataSource, DataSources.Ngo),
      searchText: searchText,
      hasFlaggedAnswers: toBoolean(search.hasFlaggedAnswers),
      level1Filter: search.level1Filter,
      level2Filter: search.level2Filter,
      level3Filter: search.level3Filter,
      level4Filter: search.level4Filter,
      level5Filter: search.level5Filter,
      pollingStationNumberFilter: search.pollingStationNumberFilter,
      followUpStatus: search.followUpStatus,
      questionsAnswered: search.questionsAnswered,
      hasNotes: toBoolean(search.hasNotes),
      hasAttachments: toBoolean(search.hasAttachments),
      tagsFilter: search.tagsFilter,
      formId: search.formId,
      fromDateFilter: search.submissionsFromDate?.toISOString(),
      toDateFilter: search.submissionsToDate?.toISOString(),
      formTypeFilter: search.formTypeFilter,
      coalitionMemberId: search.coalitionMemberId
    };

    return params;
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
