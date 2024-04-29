import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useMemo } from "react";
import { pollingStationsKeys } from "../queries.service";
import { deleteAttachment, DeleteAttachmentAPIPayload } from "../api/delete-attachment.api";
import { AttachmentApiResponse } from "../api/get-attachments.api";

export const useDeleteAttachment = (
  electionRoundId: string | undefined,
  pollingStationId: string | undefined,
  formId: string | undefined,
) => {
  const queryClient = useQueryClient();

  const getAttachmentsQK = useMemo(
    () => pollingStationsKeys.attachments(electionRoundId, pollingStationId, formId),
    [electionRoundId, pollingStationId, formId],
  );

  return useMutation({
    mutationKey: pollingStationsKeys.deleteAttachment(),
    mutationFn: async (payload: DeleteAttachmentAPIPayload) => {
      return deleteAttachment(payload);
    },
    onMutate: async (payload: DeleteAttachmentAPIPayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: getAttachmentsQK });

      // Snapshot the previous value
      const prevAttachments = queryClient.getQueryData(getAttachmentsQK);

      // Optimistically update to the new value (remove the attachment to delete)
      queryClient.setQueryData(getAttachmentsQK, (prevAttachments: AttachmentApiResponse[]) => {
        const updatedAttachments = prevAttachments.filter(
          (attachment) => attachment.id !== payload.id,
        );
        return updatedAttachments;
      });

      // Return a context object with the snapshotted value
      return { prevAttachments };
    },
    onError: (err) => {
      console.log("🔴🔴🔴 ERROR 🔴🔴🔴", err);
    },
    onSettled: () => {
      return queryClient.invalidateQueries({ queryKey: getAttachmentsQK });
    },
  });
};
