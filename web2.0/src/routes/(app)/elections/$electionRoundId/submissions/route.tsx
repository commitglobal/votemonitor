import { createFileRoute } from "@tanstack/react-router";
import { SubmissionsRoutePage } from "@/pages/NgoAdmin/Submissions/SubmissionsRoutePage";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/submissions"
)({
  component: SubmissionsRoutePage,
});
