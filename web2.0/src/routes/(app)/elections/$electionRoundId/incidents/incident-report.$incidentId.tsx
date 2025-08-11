import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/incidents/incident-report/$incidentId"
)({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div>
      Hello "/(app)/elections/$electionRoundId/quick-reports/$quickReportId"!
    </div>
  );
}
