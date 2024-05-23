import { authApi } from '@/common/auth-api';
import FormSubmissionDetails from '@/features/responses/components/FormSubmissionDetails/FormSubmissionDetails';
import { formSubmissionsByEntryKeys } from '@/features/responses/hooks/form-submissions-queries';
import type { FormSubmission } from '@/features/responses/models/form-submission';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';

export function formSubmissionDetailsQueryOptions(submissionId: string) {
  return queryOptions({
    queryKey: formSubmissionsByEntryKeys.detail(submissionId),
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
    queryClient.ensureQueryData(formSubmissionDetailsQueryOptions(submissionId)),
});
