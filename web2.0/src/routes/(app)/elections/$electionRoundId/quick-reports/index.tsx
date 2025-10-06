import { queryClient } from "@/main";
import Page from "@/pages/NgoAdmin/QuickReports/Page";
import { listQuickReportsQueryOptions } from "@/queries/quick-reports";
import { DataSource } from "@/types/common";
import { quickReportsSearchSchema } from "@/types/quick-reports";
import { createFileRoute, stripSearchParams } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/quick-reports/"
)({
  validateSearch: quickReportsSearchSchema,
  search: {
    middlewares: [
      stripSearchParams({
        dataSource: DataSource.Ngo,
        searchText: "",
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
      }),
    ],
  },
  loaderDeps: ({ search }) => ({
    ...search,
  }),
  loader: ({ deps, params: { electionRoundId } }) =>
    queryClient.prefetchQuery(
      listQuickReportsQueryOptions(electionRoundId, deps)
    ),
  component: () => <Page />,
});
