import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import Page from '@/pages/NgoAdmin/Forms/Page'
import { listFormsQueryOptions } from '@/queries/forms'
import { formSearchSchema } from '@/types/form'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/forms/'
)({
  validateSearch: formSearchSchema,
  search: {
    middlewares: [
      stripSearchParams({
        searchText: undefined,
        typeFilter: undefined,
        formStatusFilter: undefined,
      }),
    ],
  },
  loaderDeps: ({ search }) => ({
    ...search,
  }),
  loader: async ({ context, deps, params: { electionRoundId } }) => {
    await context.queryClient.ensureQueryData(
      listFormsQueryOptions(electionRoundId, deps)
    )
  },
  component: Page,
})
