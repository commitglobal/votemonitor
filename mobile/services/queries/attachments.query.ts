import { skipToken, useQuery } from "@tanstack/react-query";
import { pollingStationsKeys } from "../queries.service";
import { AttachmentApiResponse, getAttachments } from "../api/get-attachments.api";

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
    queryKey: pollingStationsKeys.attachments(electionRoundId, pollingStationId, formId),
    queryFn:
      electionRoundId && pollingStationId && formId
        ? () => getAttachments({ electionRoundId, pollingStationId, formId })
        : skipToken,
    select: (data) => mapAttachmentsToQuestionId(data),
  });
};
