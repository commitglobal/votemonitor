import Page from "@/pages/NgoAdmin/MonitoringObservers/Page";
import { monitoringObserversSearchSchema } from "@/types/monitoring-observer";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/observers/"
)({
  component: Page,
  validateSearch: monitoringObserversSearchSchema,
});
