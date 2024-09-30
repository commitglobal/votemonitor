import { authApi } from '@/common/auth-api';
import CitizenReportsFormAggregatedDetails from '@/features/responses/components/CitizenReportsFormAggregatedDetails/CitizenReportsFormAggregatedDetails';
import IssueReportsAggregatedDetails from '@/features/responses/components/IssueReportsAggregatedDetails/IssueReportsAggregatedDetails';
import { issueReportsAggregatedKeys } from '@/features/responses/hooks/issue-reports-queries';
import { SubmissionType } from '@/features/responses/models/common';
import { FormSubmissionsAggregated } from '@/features/responses/models/form-submissions-aggregated';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function issueReportsAggregatedDetailsQueryOptions(electionRoundId: string, formId: string) {
  return queryOptions({
    queryKey: issueReportsAggregatedKeys.detail(electionRoundId, formId),
    queryFn: async () => {
      const response = await authApi.get<FormSubmissionsAggregated>(
        `/election-rounds/${electionRoundId}/issue-reports/forms/${formId}:aggregated-submissions`
      );

      return {
        ...response.data,
        attachments: [
          ...response.data.attachments.map((a) => ({ ...a, submissionType: SubmissionType.IssueReport })),
        ],
        notes: [...response.data.notes.map((n) => ({ ...n, submissionType: SubmissionType.IssueReport }))],
      };
    },
    enabled: !!electionRoundId,
  });
}

export const Route = createFileRoute('/responses/issue-reports/$formId/aggregated')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: IssueReportsAggregatedDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(issueReportsAggregatedDetailsQueryOptions(electionRoundId, formId));
  },
});
