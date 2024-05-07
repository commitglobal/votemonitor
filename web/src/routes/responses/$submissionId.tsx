import { createFileRoute } from '@tanstack/react-router';
import FormSubmissionDetails from '@/features/responses/components/FormSubmissionDetails/FormSubmissionDetails';
import { queryOptions, type QueryKey, type EnsureQueryDataOptions } from '@tanstack/react-query';
import { authApi } from '@/common/auth-api';
import type { FormSubmission } from '@/features/responses/models/form-submission';
import { redirectIfNotAuth } from '@/lib/utils';

function formSubmissionQueryOptions(submissionId: string): EnsureQueryDataOptions<FormSubmission> {
  const queryKey: QueryKey = ['form-submission', submissionId];
  return queryOptions({
    queryKey,
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<FormSubmission>(
        `/election-rounds/${electionRoundId}/form-submissions/${submissionId}`
      );

      return response.data;
    },
  });
}

export const Route = createFileRoute('/responses/$submissionId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: FormSubmissionDetails,
  loader: ({ context: { queryClient }, params: { submissionId } }) =>
    queryClient.ensureQueryData(formSubmissionQueryOptions(submissionId)),
});
