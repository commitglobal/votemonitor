import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import { queryClient } from '@/main'
import { SubmissionsByEntry } from '@/pages/NgoAdmin/Submissions/by-entry'
import {
  formSubmissionsFiltersQueryOptions,
  listFormSubmissionsQueryOptions,
} from '@/queries/form-submissions'
import { pollingStationsLocationLevelsQueryOptions } from '@/queries/polling-stations'
import { DataSource } from '@/types/common'
import { formSubmissionsSearchSchema } from '@/types/forms-submission'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/submissions/'
)({
  validateSearch: formSubmissionsSearchSchema,
  search: {
    middlewares: [
      stripSearchParams({
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
      }),
    ],
  },
  loaderDeps: ({ search }) => ({
    ...search,
  }),
  loader: async ({ deps, params: { electionRoundId } }) => {
    queryClient.ensureQueryData(
      pollingStationsLocationLevelsQueryOptions(electionRoundId)
    )
    queryClient.ensureQueryData(
      formSubmissionsFiltersQueryOptions(electionRoundId, deps.dataSource)
    )
    await queryClient.prefetchQuery(
      listFormSubmissionsQueryOptions(electionRoundId, deps)
    )
  },
  component: SubmissionsByEntry,
})
