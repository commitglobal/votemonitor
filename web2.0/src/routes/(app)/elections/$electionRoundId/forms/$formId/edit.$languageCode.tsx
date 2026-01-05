import { createFileRoute, stripSearchParams } from '@tanstack/react-router'
import EditFormPage from '@/pages/NgoAdmin/EditForm/Page'
import { formSearchSchema } from '@/types/form'
import z from 'zod'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/forms/$formId/edit/$languageCode'
)({
  validateSearch: z.object({
    from: formSearchSchema.optional(),
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
      }),
    ],
  },
  component: EditFormPage,
})