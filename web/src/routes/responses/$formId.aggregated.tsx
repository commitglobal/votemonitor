import { authApi } from '@/common/auth-api';
import FormAggregatedDetails from '@/features/responses/components/FormAggregatedDetails/FormAggregatedDetails';
import { formSubmissionsAggregatedKeys } from '@/features/responses/hooks/form-submissions-queries';
import type { FormAggregated } from '@/features/responses/models/form-aggregated';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function formAggregatedDetailsQueryOptions(electionRoundId: string, formId: string) {
  return queryOptions({
    queryKey: formSubmissionsAggregatedKeys.detail(electionRoundId, formId),
    queryFn: async () => {

      const response = await authApi.get<FormAggregated>(
        `/election-rounds/${electionRoundId}/form-submissions/${formId}:aggregated`
      );

      return response.data;
    },
    enabled: !!electionRoundId
  });
}

export const Route = createFileRoute('/responses/$formId/aggregated')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: FormAggregatedDetails,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formAggregatedDetailsQueryOptions(electionRoundId, formId))
  },
});
