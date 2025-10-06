import { queryClient } from "@/main";
import Page from "@/pages/NgoAdmin/Incidents/Page";
import { listQuickReportsQueryOptions } from "@/queries/quick-reports";
import { quickReportsSearchSchema } from "@/types/quick-reports";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/incidents/"
)({
  validateSearch: quickReportsSearchSchema,
  loaderDeps: ({ search }) => ({
    ...search,
  }),
  loader: ({ deps, params: { electionRoundId } }) =>
    queryClient.prefetchQuery(
      listQuickReportsQueryOptions(electionRoundId, deps)
    ),
  component: () => <Page />,
});
