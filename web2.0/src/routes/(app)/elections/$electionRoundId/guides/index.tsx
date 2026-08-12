import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import Page from '@/pages/NgoAdmin/GuidesObservers/Page'
import { listGuidesObserversQueryOptions } from '@/queries/guides-observers'
import { guidesObserversSearchSchema } from '@/types/guides-observer'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/guides/'
)({
  validateSearch: guidesObserversSearchSchema,
  search: {
    // Keep the url clean while nothing is filtered.
    middlewares: [
      stripSearchParams({
        searchText: undefined,
        guideTypeFilter: undefined,
      }),
    ],
  },
  loader: async ({ context, params: { electionRoundId } }) => {
    // The list is fetched without the search params on purpose: filtering,
    // sorting and paging all happen on the client, so the same payload serves
    // every combination of them.
    await context.queryClient.ensureQueryData(
      listGuidesObserversQueryOptions(electionRoundId)
    )
  },
  component: Page,
})
