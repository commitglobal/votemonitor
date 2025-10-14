import z from 'zod'
import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import { queryClient } from '@/main'
import { Page as SubmissionDetailsPage } from '@/pages/NgoAdmin/Submissions/SubmissionDetails'
import { getFormSubmissionDetailsQueryOptions } from '@/queries/form-submissions'
import { DataSource } from '@/types/common'
import { formSubmissionsSearchSchema } from '@/types/forms-submission'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/submissions/$submissionId/'
)({
  validateSearch: z.object({
    from: formSubmissionsSearchSchema.optional(),
    formLanguage: z.string().optional(),
  }),
  search: {
    middlewares: [
      stripSearchParams({
        from: {
          formTypeFilter: undefined,
          level1Filter: '',
          level2Filter: '',
          level3Filter: '',
          level4Filter: '',
          level5Filter: '',
          pollingStationNumberFilter: '',
          hasFlaggedAnswers: '',
          monitoringObserverId: '',
          tagsFilter: [],
          followUpStatus: undefined,
          questionsAnswered: undefined,
          hasNotes: '',
          hasAttachments: '',
          formId: '',
          submissionsFromDate: undefined,
          submissionsToDate: undefined,
          coalitionMemberId: '',
          dataSource: DataSource.Ngo,
          sortColumnName: undefined,
          sortOrder: undefined,
          pageNumber: 1,
          pageSize: 25,
        },
      }),
    ],
  },
  loader: ({ params: { electionRoundId, submissionId } }) =>
    queryClient.ensureQueryData(
      getFormSubmissionDetailsQueryOptions(electionRoundId, submissionId)
    ),
  component: SubmissionDetailsPage,
})
