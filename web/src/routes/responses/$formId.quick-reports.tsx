import { type EnsureQueryDataOptions, type QueryKey, queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';
import { authApi } from '@/common/auth-api';
import QuickReportDetails from '@/features/responses/components/QuickReportDetails/QuickReportDetails';
import type { QuickReport } from '@/features/responses/models/quick-report';
import { redirectIfNotAuth } from '@/lib/utils';

function quickReportDetailsQueryOptions(formId: string): EnsureQueryDataOptions<QuickReport> {
  const queryKey: QueryKey = ['quick-report', formId];
  return queryOptions({
    queryKey,
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<QuickReport>(`/election-rounds/${electionRoundId}/quick-reports/${formId}`);

      return response.data;
    },
  });
}

export const Route = createFileRoute('/responses/$formId/quick-reports')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: QuickReportDetails,
  loader: ({ context: { queryClient }, params: { formId } }) =>
    queryClient.ensureQueryData(quickReportDetailsQueryOptions(formId)),
});
