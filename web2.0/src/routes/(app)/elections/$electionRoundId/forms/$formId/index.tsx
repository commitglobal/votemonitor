import z from 'zod'
import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import { queryClient } from '@/main'
import PreviewFormPage from '@/pages/NgoAdmin/PreviewForm/Page'
import { getFormDetailsQueryOptions } from '@/queries/forms'
import { formSearchSchema } from '@/types/form'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/forms/$formId/'
)({
  validateSearch: z.object({
    from: formSearchSchema.optional(),
    formLanguage: z.string().optional(),
  }),
  search: {
    middlewares: [
      stripSearchParams({
        from: {
          searchText: undefined,
          typeFilter: undefined,
          formStatusFilter: undefined,
          pageNumber: 1,
          pageSize: 25,
        },
        formLanguage: undefined,
      }),
    ],
  },
  loaderDeps: ({ search }) => ({
    ...search,
  }),
  loader: ({ params: { electionRoundId, formId } }) =>
    queryClient.prefetchQuery(
      getFormDetailsQueryOptions(electionRoundId, formId)
    ),
  component: PreviewFormPage,
})
