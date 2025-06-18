import { authApi } from '@/common/auth-api';
import IncidentReportsAggregatedDetails from '@/features/responses/components/IncidentReportsAggregatedDetails/IncidentReportsAggregatedDetails';
import { incidentReportsAggregatedKeys } from '@/features/responses/hooks/incident-reports-queries';
import { SubmissionType } from '@/features/responses/models/common';
import { FormSubmissionsAggregated } from '@/features/responses/models/form-submissions-aggregated';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function incidentReportsAggregatedDetailsQueryOptions(electionRoundId: string, formId: string) {
  return queryOptions({
    queryKey: incidentReportsAggregatedKeys.detail(electionRoundId, formId),
    queryFn: async () => {
      const response = await authApi.get<FormSubmissionsAggregated>(
        `/election-rounds/${electionRoundId}/incident-reports/forms/${formId}:aggregated-submissions`
      );

      return {
        ...response.data,
        attachments: [
          ...response.data.attachments.map((a) => ({ ...a, submissionType: SubmissionType.IncidentReport })),
        ],
        notes: [...response.data.notes.map((n) => ({ ...n, submissionType: SubmissionType.IncidentReport }))],
      };
    },
    enabled: !!electionRoundId,
  });
}

export const Route = createFileRoute('/(app)/responses/incident-reports/$formId/aggregated')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: IncidentReportsAggregatedDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(incidentReportsAggregatedDetailsQueryOptions(electionRoundId, formId));
  },
});
