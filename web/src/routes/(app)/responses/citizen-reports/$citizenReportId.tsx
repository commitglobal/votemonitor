import API from '@/services/api';
import CitizenReportDetails from '@/features/responses/components/CitizenReportDetails/CitizenReportDetails';
import { citizenReportKeys } from '@/features/responses/hooks/citizen-reports';
import { CitizenReport } from '@/features/responses/models/citizen-report';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function citizenReportDetailsQueryOptions(electionRoundId: string, citizenReportId: string) {
  return queryOptions({
    queryKey: citizenReportKeys.detail(electionRoundId, citizenReportId),
    queryFn: async () => {
      const response = await API.get<CitizenReport>(
        `/election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}`
      );

      return response.data;
    },
    enabled: !!electionRoundId,
  });
}

export const Route = createFileRoute('/(app)/responses/citizen-reports/$citizenReportId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: CitizenReportDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { citizenReportId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(citizenReportDetailsQueryOptions(electionRoundId, citizenReportId));
  },
});
