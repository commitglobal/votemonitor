import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/push-messages/$pushMessageId',
)({
  component: RouteComponent,
})

function RouteComponent() {
  return (
    <div>
      Hello "/(app)/elections/$electionRoundId/push-messages/$pushMessageId"!
    </div>
  )
}
