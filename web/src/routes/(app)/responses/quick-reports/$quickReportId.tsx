import API from '@/services/api';
import QuickReportDetails from '@/features/responses/components/QuickReportDetails/QuickReportDetails';
import { quickReportKeys } from '@/features/responses/hooks/quick-reports';
import type { QuickReport } from '@/features/responses/models/quick-report';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function quickReportDetailsQueryOptions(electionRoundId: string, quickReportId: string) {
  return queryOptions({
    queryKey: quickReportKeys.detail(electionRoundId, quickReportId),
    queryFn: async () => {
      const response = await API.get<QuickReport>(`/election-rounds/${electionRoundId}/quick-reports/${quickReportId}`);

      return response.data;
    },
    enabled: !!electionRoundId,
  });
}

export const Route = createFileRoute('/(app)/responses/quick-reports/$quickReportId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: QuickReportDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { quickReportId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(quickReportDetailsQueryOptions(electionRoundId, quickReportId));
  },
});
