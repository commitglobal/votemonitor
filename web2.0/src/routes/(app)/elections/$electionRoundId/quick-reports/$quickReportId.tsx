import QuickReportsDetails from "@/pages/NgoAdmin/QuickReportsDetails";
import { quickReportDetailsQueryOptions } from "@/queries/quick-reports";
import { DataSource, SortOrder } from "@/types/common";
import { quickReportsSearchSchema } from "@/types/quick-reports";
import { createFileRoute, stripSearchParams } from "@tanstack/react-router";
import z from "zod";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/quick-reports/$quickReportId"
)({
  validateSearch: z.object({
    from: quickReportsSearchSchema.optional(),
  }),
  search: {
    middlewares: [
      stripSearchParams({
        from: {
          dataSource: DataSource.Ngo,
          searchText: "",
          sortColumnName: "",
          sortOrder: SortOrder.Asc,
          pageNumber: 1,
          pageSize: 25,
          level1Filter: "",
          level2Filter: "",
          level3Filter: "",
          level4Filter: "",
          level5Filter: "",
          pollingStationNumberFilter: "",
          quickReportFollowUpStatus: undefined,
          quickReportLocationType: undefined,
          incidentCategory: undefined,
          coalitionMemberId: "",
          pollingStationId: "",
          observerId: "",
        },
      }),
    ],
  },
  loader: (opts) =>
    opts.context.queryClient.ensureQueryData(
      quickReportDetailsQueryOptions(
        opts.params.electionRoundId,
        opts.params.quickReportId
      )
    ),
  component: QuickReportsDetails,
});
