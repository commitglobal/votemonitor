import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/forms/$formId/edit/$languageCode"
)({
  component: RouteComponent,
});

function RouteComponent() {
  return <div>Hello "/(app)/elections/$electionRoundId/forms/$formId"!</div>;
}
