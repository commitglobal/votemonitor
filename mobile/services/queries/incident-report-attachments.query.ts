import { skipToken, useQuery } from "@tanstack/react-query";
import { getIncidentReportAttachments, IncidentReportAttachmentApiResponse } from "../api/get-incident-report-attachments.api";

export const incidentReportAttachmentsKeys = {
  all: ["incident-report-attachments"] as const,
  attachments: (
    electionRoundId: string | undefined,
    incidentReportId: string | undefined,
  ) =>
    [
      ...incidentReportAttachmentsKeys.all,
      "electionRoundId",
      electionRoundId,
      "incidentReportId",
      incidentReportId,
    ] as const,
  addAttachmentMutation: () => [...incidentReportAttachmentsKeys.all, "add"] as const,
  deleteAttachment: () => [...incidentReportAttachmentsKeys.all, "delete"] as const,
};

// TODO: make generic fn
const mapAttachmentsToQuestionId = (attachments: IncidentReportAttachmentApiResponse[]) => {
  return attachments?.reduce(
    (acc: Record<string, IncidentReportAttachmentApiResponse[]>, curr: IncidentReportAttachmentApiResponse) => {
      if (!acc[curr.questionId]) {
        acc[curr.questionId] = [];
      }
      acc[curr.questionId].push(curr);
      return acc;
    },
    {},
  );
};

export const useIncidentReportAttachments = (
  electionRoundId: string | undefined,
  incidentReportId: string | undefined,
) => {
  return useQuery({
    queryKey: incidentReportAttachmentsKeys.attachments(electionRoundId, incidentReportId),
    queryFn:
      electionRoundId && incidentReportId
        ? () => getIncidentReportAttachments({ electionRoundId, incidentReportId })
        : skipToken,
    select: (data) => mapAttachmentsToQuestionId(data),
  });
};
