import z from 'zod'
import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import { queryClient } from '@/main'
import { AggregatedSubmissionsPage } from '@/pages/NgoAdmin/Submissions/AggregatedSubmissionsPage'
import { formSubmissionsFiltersQueryOptions } from '@/queries/form-submissions'
import { getSubmissionsAggregatedDetailsQueryOptions } from '@/queries/form-submissions-aggregated'
import { pollingStationsLocationLevelsQueryOptions } from '@/queries/polling-stations'
import { DataSource } from '@/types/common'
import { formSubmissionsSearchSchema } from '@/types/forms-submission'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/submissions/by-form/$formId'
)({
  validateSearch: z.object({
    ...formSubmissionsSearchSchema.shape,
    formLanguage: z.string().optional(),
  }),
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
  loader: async ({ deps, params: { electionRoundId, formId } }) => {
    queryClient.prefetchQuery(
      pollingStationsLocationLevelsQueryOptions(electionRoundId)
    )
    queryClient.prefetchQuery(
      formSubmissionsFiltersQueryOptions(electionRoundId, deps.dataSource)
    )
    await queryClient.ensureQueryData(
      getSubmissionsAggregatedDetailsQueryOptions(electionRoundId, formId, deps)
    )
  },
  component: AggregatedSubmissionsPage,
})
