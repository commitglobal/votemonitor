import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute(
  '/(app)/elections/$electionRoundId/quick-reports/',
)({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/(app)/elections/$electionRoundId/quick-reports"!</div>
}
