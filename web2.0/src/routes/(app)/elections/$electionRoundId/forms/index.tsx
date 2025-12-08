import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import { queryClient } from '@/main'
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
  loader: ({ deps, params: { electionRoundId } }) =>
    queryClient.prefetchQuery(listFormsQueryOptions(electionRoundId, deps)),
  component: Page,
})
