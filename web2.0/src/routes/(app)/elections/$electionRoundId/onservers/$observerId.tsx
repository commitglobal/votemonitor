import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/onservers/$observerId',
)({
  component: RouteComponent,
})

function RouteComponent() {
  return (
    <div>Hello "/(app)/elections/$electionRoundId/onservers/$observerId"!</div>
  )
}
