import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/observers"
)({
  component: RouteComponent,
});

function RouteComponent() {
  return <div>Hello "/(app)/elections/$electionRoundId/observers"!</div>;
}
