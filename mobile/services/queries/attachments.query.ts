import { skipToken, useQuery } from "@tanstack/react-query";
import { AttachmentApiResponse, getAttachments } from "../api/get-attachments.api";

export const AttachmentsKeys = {
  all: ["attachments"] as const,
  attachments: (
    electionRoundId: string | undefined,
    pollingStationId: string | undefined,
    formId: string | undefined,
  ) =>
    [
      ...AttachmentsKeys.all,
      "electionRoundId",
      electionRoundId,
      "pollingStationId",
      pollingStationId,
      "formId",
      formId,
    ] as const,
  addAttachmentMutation: () => [...AttachmentsKeys.all, "add"] as const,
  deleteAttachment: () => [...AttachmentsKeys.all, "delete"] as const,
};

// TODO: make generic fn
const mapAttachmentsToQuestionId = (attachments: AttachmentApiResponse[]) => {
  return attachments?.reduce(
    (acc: Record<string, AttachmentApiResponse[]>, curr: AttachmentApiResponse) => {
      if (!acc[curr.questionId]) {
        acc[curr.questionId] = [];
      }
      acc[curr.questionId].push(curr);
      return acc;
    },
    {},
  );
};

export const useAttachments = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
) => {
  return useQuery({
    queryKey: AttachmentsKeys.attachments(electionRoundId, pollingStationId, formId),
    queryFn:
      electionRoundId && pollingStationId && formId
        ? () => getAttachments({ electionRoundId, pollingStationId, formId })
        : skipToken,
    select: (data) => mapAttachmentsToQuestionId(data),
  });
};
