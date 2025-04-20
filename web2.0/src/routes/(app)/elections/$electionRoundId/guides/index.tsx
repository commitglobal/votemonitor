import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/guides/"
)({
  component: RouteComponent,
});

function RouteComponent() {
  return <div>Hello "/(app)/elections/$electionRoundId/guides/"!</div>;
}
