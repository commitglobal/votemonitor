import { queryClient } from "@/main";
import { SubmissionsByEntry } from "@/pages/NgoAdmin/Submissions/by-entry";
import { listFormSubmissionsQueryOptions } from "@/queries/form-submissions";
import { formSubmissionsSearchSchema } from "@/types/forms-submission";
import { createFileRoute, stripSearchParams } from "@tanstack/react-router";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/submissions/"
)({
  validateSearch: formSubmissionsSearchSchema,
  search: {
    middlewares: [
      stripSearchParams({
        formTypeFilter: undefined,
        level1Filter: "",
        level2Filter: "",
        level3Filter: "",
        level4Filter: "",
        level5Filter: "",
        pollingStationNumberFilter: "",
        hasFlaggedAnswers: "",
        monitoringObserverId: "",
        tagsFilter: [],
        followUpStatus: undefined,
        questionsAnswered: undefined,
        hasNotes: "",
        hasAttachments: "",
        formId: "",
        submissionsFromDate: undefined,
        submissionsToDate: undefined,
        coalitionMemberId: "",
      }),
    ],
  },
  loaderDeps: ({ search }) => ({
    ...search,
  }),
  loader: ({ deps, params: { electionRoundId } }) =>
    queryClient.prefetchQuery(
      listFormSubmissionsQueryOptions(electionRoundId, deps)
    ),
  component: SubmissionsByEntry,
});
