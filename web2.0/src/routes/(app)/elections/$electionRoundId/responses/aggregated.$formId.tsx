import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/responses/aggregated/$formId"
)({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div>
      Hello "/(app)/elections/$electionRoundId/responses/aggregated/$formId"!
    </div>
  );
}
