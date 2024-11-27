import { DataSources, FormSubmissionFollowUpStatus, FunctionComponent, QuestionsAnswered } from '@/common/types';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { CardContent } from '@/components/ui/card';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { getValueOrDefault, toBoolean } from '@/lib/utils';
import { Route } from '@/routes/responses';
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useFormSubmissionsByEntry } from '../../hooks/form-submissions-queries';
import { useFormSubmissionsByEntryColumns } from '../../store/column-visibility';
import { formSubmissionsByEntryColumnDefs } from '../../utils/column-defs';

type FormSubmissionsByEntryTableProps = {
  searchText: string;
};

export interface FormSubmissionsSearchRequest{
  dataSource: DataSources;
  searchText: string | undefined;
  formTypeFilter: string | undefined;
  hasFlaggedAnswers: boolean | undefined;
  level1Filter: string | undefined;
  level2Filter: string | undefined;
  level3Filter: string | undefined;
  level4Filter: string | undefined;
  level5Filter: string | undefined;
  pollingStationNumberFilter: string | undefined;
  followUpStatus: FormSubmissionFollowUpStatus | undefined;
  questionsAnswered: QuestionsAnswered | undefined;
  hasNotes: boolean | undefined;
  hasAttachments: boolean | undefined;
  tagsFilter: string[] | undefined;
  formId: string | undefined;
  fromDateFilter: string | undefined;
  toDateFilter: string | undefined;
  coalitionMemberId: string | undefined;
}

export function FormSubmissionsByEntryTable({ searchText }: FormSubmissionsByEntryTableProps): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const columnsVisibility = useFormSubmissionsByEntryColumns();

  const queryParams = useMemo(() => {
    const params: FormSubmissionsSearchRequest = {
      dataSource: getValueOrDefault(search.dataSource, DataSources.Ngo),
      searchText: searchText,
      formTypeFilter: debouncedSearch.formTypeFilter,
      hasFlaggedAnswers: toBoolean(debouncedSearch.hasFlaggedAnswers),
      level1Filter: debouncedSearch.level1Filter,
      level2Filter: debouncedSearch.level2Filter,
      level3Filter: debouncedSearch.level3Filter,
      level4Filter: debouncedSearch.level4Filter,
      level5Filter: debouncedSearch.level5Filter,
      pollingStationNumberFilter: debouncedSearch.pollingStationNumberFilter,
      followUpStatus: debouncedSearch.followUpStatus,
      questionsAnswered: debouncedSearch.questionsAnswered,
      hasNotes: toBoolean(debouncedSearch.hasNotes),
      hasAttachments: toBoolean(debouncedSearch.hasAttachments),
      tagsFilter: debouncedSearch.tagsFilter,
      formId: debouncedSearch.formId,
      fromDateFilter: debouncedSearch.submissionsFromDate?.toISOString(),
      toDateFilter: debouncedSearch.submissionsToDate?.toISOString(),
      coalitionMemberId: debouncedSearch.coalitionMemberId
    };

    return params;
  }, [searchText, debouncedSearch]);

  const navigateToFormSubmission = useCallback(
    (submissionId: string) => {
      void navigate({ to: '/responses/form-submissions/$submissionId', params: { submissionId } });
    },
    [navigate]
  );

  return (
    <CardContent>
      <QueryParamsDataTable
        columnVisibility={columnsVisibility}
        columns={formSubmissionsByEntryColumnDefs}
        useQuery={(params) => useFormSubmissionsByEntry(currentElectionRoundId, params)}
        queryParams={queryParams}
        onRowClick={navigateToFormSubmission}
      />
    </CardContent>
  );
}
