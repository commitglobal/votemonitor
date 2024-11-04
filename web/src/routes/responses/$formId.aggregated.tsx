import { authApi } from '@/common/auth-api';
import { FormSubmissionFollowUpStatus, QuestionsAnswered } from '@/common/types';
import FormSubmissionsAggregatedDetails from '@/features/responses/components/FormSubmissionsAggregatedDetails/FormSubmissionsAggregatedDetails';
import { formSubmissionsAggregatedKeys } from '@/features/responses/hooks/form-submissions-queries';
import { SubmissionType } from '@/features/responses/models/common';
import { FormSubmissionsAggregated } from '@/features/responses/models/form-submissions-aggregated';
import { buildURLSearchParams, redirectIfNotAuth } from '@/lib/utils';
import { queryOptions } from '@tanstack/react-query';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

export function formAggregatedDetailsQueryOptions(
  electionRoundId: string,
  formId: string,
  params: SubmissionsAggregatedByFormParams
) {
  return queryOptions({
    queryKey: formSubmissionsAggregatedKeys.detail(electionRoundId, formId, params),
    queryFn: async () => {
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<FormSubmissionsAggregated>(
        `/election-rounds/${electionRoundId}/form-submissions/${formId}:aggregated`,
        {
          params: searchParams,
        }
      );

      return {
        ...response.data,
        attachments: [
          ...response.data.attachments.map((a) => ({ ...a, submissionType: SubmissionType.FormSubmission })),
        ],
        notes: [...response.data.notes.map((n) => ({ ...n, submissionType: SubmissionType.FormSubmission }))],
      };
    },
    enabled: !!electionRoundId,
  });
}

export const SubmissionsAggregatedByFormSchema = z.object({
  level1Filter: z.string().catch('').optional(),
  level2Filter: z.string().catch('').optional(),
  level3Filter: z.string().catch('').optional(),
  level4Filter: z.string().catch('').optional(),
  level5Filter: z.string().catch('').optional(),
  pollingStationNumberFilter: z.string().catch('').optional(),
  hasFlaggedAnswers: z.string().catch('').optional(),
  tagsFilter: z.array(z.string()).optional().catch([]).optional(),
  followUpStatus: z.nativeEnum(FormSubmissionFollowUpStatus).optional(),
  questionsAnswered: z.nativeEnum(QuestionsAnswered).optional(),
  hasNotes: z.string().catch('').optional(),
  hasAttachments: z.string().catch('').optional(),
  formIsCompleted: z.string().catch('').optional(),
  submissionsFromDate: z.coerce.date().optional(),
  submissionsToDate: z.coerce.date().optional(),
});

export type SubmissionsAggregatedByFormParams = z.infer<typeof SubmissionsAggregatedByFormSchema>;


export const Route = createFileRoute('/responses/$formId/aggregated')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: FormSubmissionsAggregatedDetails,
  loaderDeps: ({ search }) => ({ search }),
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId }, deps : {
    search: queryParams
  } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formAggregatedDetailsQueryOptions(electionRoundId, formId, queryParams));
  },
  validateSearch: SubmissionsAggregatedByFormSchema,
});
