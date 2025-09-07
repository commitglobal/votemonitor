import { skipToken, useQuery } from "@tanstack/react-query";
import { AttachmentApiResponse, getAttachments } from "../api/get-attachments.api";

export const AttachmentsKeys = {
  all: ["attachments"] as const,
  attachments: (electionRoundId: string | undefined, submissionId: string | undefined) =>
    [
      ...AttachmentsKeys.all,
      "electionRoundId",
      electionRoundId,
      "submissionId",
      submissionId,
    ] as const,
  addAttachmentMutation: () => [...AttachmentsKeys.all, "add"] as const,
  addAttachmentCompleteMutation: () => [...AttachmentsKeys.all, "complete"] as const,
  addAttachmentAbortMutation: () => [...AttachmentsKeys.all, "abort"] as const,
  addS3ChunkMutation: () => [...AttachmentsKeys.all, "addS3Chunk"] as const,
  deleteAttachment: () => [...AttachmentsKeys.all, "delete"] as const,
};

export const GuidesKeys = {
  guides: (electionRoundId: string | undefined) =>
    ["guides", "electionRoundId", electionRoundId] as const,
  citizenGuides: (electionRoundId: string | undefined) =>
    ["citizen", "guides", "electionRoundId", electionRoundId] as const,
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
  submissionId: string | undefined,
) => {
  return useQuery({
    queryKey: AttachmentsKeys.attachments(electionRoundId, submissionId),
    queryFn:
      electionRoundId && submissionId
        ? () => getAttachments({ electionRoundId, submissionId })
        : skipToken,
    select: (data) => mapAttachmentsToQuestionId(data),
  });
};
