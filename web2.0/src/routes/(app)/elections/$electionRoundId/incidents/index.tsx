import Page from "@/pages/NgoAdmin/Incidents/Page";
import { quickReportsSearchSchema } from "@/types/quick-reports";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/incidents/"
)({
  component: () => <Page />,
  validateSearch: quickReportsSearchSchema,
});
