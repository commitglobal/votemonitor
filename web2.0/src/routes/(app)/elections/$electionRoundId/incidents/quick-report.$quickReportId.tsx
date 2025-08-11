import QuickReportsDetails from "@/pages/NgoAdmin/QuickReportsDetails";
import { quickReportDetailsQueryOptions } from "@/queries/quick-reports";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/incidents/quick-report/$quickReportId"
)({
  loader: (opts) =>
    opts.context.queryClient.ensureQueryData(
      quickReportDetailsQueryOptions(
        opts.params.electionRoundId,
        opts.params.quickReportId
      )
    ),
  component: QuickReportsDetails,
});
