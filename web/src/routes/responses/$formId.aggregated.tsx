import { authApi } from '@/common/auth-api';
import FormAggregatedDetails from '@/features/responses/components/FormAggregatedDetails/FormAggregatedDetails';
import { formSubmissionsAggregtedKeys } from '@/features/responses/hooks/form-submissions-queries';
import type { FormAggregated } from '@/features/responses/models/form-aggregated';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function formAggregatedDetailsQueryOptions(formId: string) {
  return queryOptions({
    queryKey: formSubmissionsAggregtedKeys.detail(formId),
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<FormAggregated>(
        `/election-rounds/${electionRoundId}/form-submissions/${formId}:aggregated`
      );

      return response.data;
    },
  });
}

export const Route = createFileRoute('/responses/$formId/aggregated')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: FormAggregatedDetails,
  loader: ({ context: { queryClient }, params: { formId } }) =>
    queryClient.ensureQueryData(formAggregatedDetailsQueryOptions(formId)),
});
