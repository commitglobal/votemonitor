import { queryClient } from "@/main";
import Page from "@/pages/NgoAdmin/QuickReports/Page";
import { quickReportsQueryOptions } from "@/queries/quick-reports";
import { DataSource, SortOrder } from "@/types/common";
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
        followUpStatus: undefined,
        locationType: undefined,
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
    queryClient.prefetchQuery(quickReportsQueryOptions(electionRoundId, deps)),
  component: () => <Page />,
});
