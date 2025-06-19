import API from '@/services/api';
import IncidentReportDetails from '@/features/responses/components/IncidentReportDetails/IncidentReportDetails';
import { incidentReportsByEntryKeys } from '@/features/responses/hooks/incident-reports-queries';
import { IncidentReport } from '@/features/responses/models/incident-report';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function incidentReportDetailsQueryOptions(electionRoundId: string, incidentReportId: string) {
  return queryOptions({
    queryKey: incidentReportsByEntryKeys.detail(electionRoundId, incidentReportId),
    queryFn: async () => {
      const response = await API.get<IncidentReport>(
        `/election-rounds/${electionRoundId}/incident-reports/${incidentReportId}`
      );

      return response.data;
    },
    enabled: !!electionRoundId,
  });
}

export const Route = createFileRoute('/(app)/responses/incident-reports/$incidentReportId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: IncidentReportDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { incidentReportId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(incidentReportDetailsQueryOptions(electionRoundId, incidentReportId));
  },
});
