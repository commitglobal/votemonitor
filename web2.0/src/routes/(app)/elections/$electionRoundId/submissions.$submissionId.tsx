import { queryClient } from "@/main";
import { getFormSubmissionDetailsQueryOptions } from "@/queries/form-submissions";
import { formSubmissionsSearchSchema } from "@/types/forms-submission";
import { createFileRoute, stripSearchParams } from "@tanstack/react-router";
import z from "zod";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/submissions/$submissionId"
)({
  validateSearch: z.object({
    from: formSubmissionsSearchSchema.optional(),
  }),
  search: {
    middlewares: [
      stripSearchParams({
        from: {
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
        },
      }),
    ],
  },
  loader: ({ params: { electionRoundId, submissionId } }) =>
    queryClient.ensureQueryData(
      getFormSubmissionDetailsQueryOptions(electionRoundId, submissionId)
    ),
  component: RouteComponent,
});

function RouteComponent() {
  const data = Route.useParams();
  return <pre>{JSON.stringify(data, null, 2)}</pre>;
}
