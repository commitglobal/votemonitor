import Page from "@/pages/NgoAdmin/MonitoringObservers/Page";
import { monitoringObserversSearchSchema } from "@/types/monitoring-observer";
import { createFileRoute } from "@tanstack/react-router";
import { zodValidator } from "@tanstack/zod-adapter";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/observers/"
)({
  validateSearch: zodValidator(monitoringObserversSearchSchema),
  component: Page,
});
