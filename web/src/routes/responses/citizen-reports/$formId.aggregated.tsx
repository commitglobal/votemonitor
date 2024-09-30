import { authApi } from '@/common/auth-api';
import CitizenReportsFormAggregatedDetails from '@/features/responses/components/CitizenReportsFormAggregatedDetails/CitizenReportsFormAggregatedDetails';
import { citizenReportsAggregatedKeys } from '@/features/responses/hooks/citizen-reports';
import { CitizenReportsFormAggregated } from '@/features/responses/models/citizen-reports-form-aggregated';
import { SubmissionType } from '@/features/responses/models/common';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function citizenReportsAggregatedDetailsQueryOptions(electionRoundId: string, formId: string) {
  return queryOptions({
    queryKey: citizenReportsAggregatedKeys.detail(electionRoundId, formId),
    queryFn: async () => {
      const response = await authApi.get<CitizenReportsFormAggregated>(
        `/election-rounds/${electionRoundId}/citizen-reports/forms/${formId}:aggregated-submissions`
      );

      return {
        ...response.data,
        attachments: [
          ...response.data.attachments.map((a) => ({ ...a, submissionType: SubmissionType.CitizenReport })),
        ],
        notes: [...response.data.notes.map((n) => ({ ...n, submissionType: SubmissionType.CitizenReport }))],
      };
    },
    enabled: !!electionRoundId,
  });
}

export const Route = createFileRoute('/responses/citizen-reports/$formId/aggregated')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CitizenReportsFormAggregatedDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(citizenReportsAggregatedDetailsQueryOptions(electionRoundId, formId));
  },
});
