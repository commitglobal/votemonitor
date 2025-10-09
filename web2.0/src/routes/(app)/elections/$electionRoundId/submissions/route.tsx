import { createFileRoute, Outlet, useMatchRoute } from "@tanstack/react-router";
import { SubmissionsRoutePage } from "@/pages/NgoAdmin/Submissions/SubmissionsRoutePage";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/submissions"
)({
  component: RouteComponent,
});

function RouteComponent() {
  const matchRoute = useMatchRoute();
  const isDetailRoute = !!matchRoute({
    to: "/elections/$electionRoundId/submissions/$submissionId",
    fuzzy: false,
  });

  return isDetailRoute ? <Outlet /> : <SubmissionsRoutePage />;
}
