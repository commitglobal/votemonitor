import { authApi } from '@/common/auth-api';
import IssueReportDetails from '@/features/responses/components/IssueReportDetails/IssueReportDetails';
import { issueReportsByEntryKeys } from '@/features/responses/hooks/issue-reports-queries';
import { IssueReport } from '@/features/responses/models/issue-report';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function issueReportDetailsQueryOptions(electionRoundId: string, issueReportId: string) {
  return queryOptions({
    queryKey: issueReportsByEntryKeys.detail(electionRoundId, issueReportId),
    queryFn: async () => {
      const response = await authApi.get<IssueReport>(
        `/election-rounds/${electionRoundId}/issue-reports/${issueReportId}`
      );

      return response.data;
    },
    enabled: !!electionRoundId,
  });
}

export const Route = createFileRoute('/responses/issue-reports/$issueReportId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: IssueReportDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { issueReportId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(issueReportDetailsQueryOptions(electionRoundId, issueReportId));
  },
});
