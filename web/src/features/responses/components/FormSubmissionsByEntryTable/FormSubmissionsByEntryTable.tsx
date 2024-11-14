import type { FormSubmissionFollowUpStatus, FunctionComponent, QuestionsAnswered } from '@/common/types';
import { CardContent } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { Route } from '@/routes/responses';
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useFormSubmissionsByEntry } from '../../hooks/form-submissions-queries';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { useFormSubmissionsByEntryColumns } from '../../store/column-visibility';
import { formSubmissionsByEntryColumnDefs } from '../../utils/column-defs';

type FormSubmissionsByEntryTableProps = {
  searchText: string;
};

export interface FormSubmissionsSearchRequest{
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
}


export function FormSubmissionsByEntryTable({ searchText }: FormSubmissionsByEntryTableProps): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const columnsVisibility = useFormSubmissionsByEntryColumns();

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', searchText],
      ['formTypeFilter', debouncedSearch.formTypeFilter],
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
      ['tagsFilter', debouncedSearch.tagsFilter],
      ['formId', debouncedSearch.formId],
      ['fromDateFilter', debouncedSearch.submissionsFromDate?.toISOString()],
      ['toDateFilter', debouncedSearch.submissionsToDate?.toISOString()],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormSubmissionsSearchRequest;
  }, [searchText, debouncedSearch]);

  const navigateToFormSubmission = useCallback(
    (submissionId: string) => {
      void navigate({ to: '/responses/$submissionId', params: { submissionId } });
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
