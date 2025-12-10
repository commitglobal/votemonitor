import { createFileRoute } from '@tanstack/react-router'
import WizardPage from '@/pages/NgoAdmin/FormsWizzard/Page'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/forms/wizard'
)({
  component: WizardPage,
})
