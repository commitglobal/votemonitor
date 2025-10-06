import { SubmissionsByForm } from "@/pages/NgoAdmin/Submissions/by-form";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/submissions/by-form"
)({
  component: SubmissionsByForm,
});
