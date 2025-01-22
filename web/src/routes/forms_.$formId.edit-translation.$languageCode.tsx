import EditFormTranslation from '@/features/forms/components/EditFormTranslation/EditFormTranslation'
import { formDetailsQueryOptions } from '@/features/forms/queries'
import { redirectIfNotAuth } from '@/lib/utils'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute(
  '/forms_/$formId/edit-translation/$languageCode',
)({
  component: Edit,
  loader: ({
    context: { queryClient, currentElectionRoundContext },
    params: { formId },
  }) => {
    const electionRoundId =
      currentElectionRoundContext.getState().currentElectionRoundId

    return queryClient.ensureQueryData(
      formDetailsQueryOptions(electionRoundId, formId),
    )
  },
  beforeLoad: () => {
    redirectIfNotAuth()
  },
})

function Edit() {
  return (
    <div className="p-2 flex-1">
      <EditFormTranslation />
    </div>
  )
}
