import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useMemo } from "react";
import { deleteAttachment } from "../../api/delete-attachment.api";
import { AttachmentApiResponse } from "../../api/get-attachments.api";
import { AttachmentsKeys } from "../../queries/attachments.query";

export const useDeleteAttachment = (
  electionRoundId: string | undefined,
  submissionId: string | undefined,
  scopeId: string,
) => {
  const queryClient = useQueryClient();

  const getAttachmentsQK = useMemo(
    () => AttachmentsKeys.attachments(electionRoundId, submissionId),
    [electionRoundId, submissionId],
  );

  return useMutation({
    mutationKey: AttachmentsKeys.deleteAttachment(),
    scope: {
      id: scopeId,
    },
    mutationFn: async (payload: AttachmentApiResponse) => {
      return deleteAttachment({ electionRoundId: electionRoundId as string, ...payload });
    },
    onMutate: async (payload: AttachmentApiResponse) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: getAttachmentsQK });

      // Snapshot the previous value
      const prevAttachments = queryClient.getQueryData<AttachmentApiResponse[]>(getAttachmentsQK);

      // Optimistically update to the new value (remove the attachment to delete)
      queryClient.setQueryData(getAttachmentsQK, (prevAttachments: AttachmentApiResponse[]) => {
        const updatedAttachments = prevAttachments?.filter(
          (attachment) => attachment.id !== payload.id,
        );
        return updatedAttachments;
      });

      if (payload.isNotSynched) {
        const mutationsToRemove = queryClient
          .getMutationCache()
          .findAll({
            mutationKey: AttachmentsKeys.all,
          })
          .filter((mutation) => {
            // @ts-ignore
            return mutation?.state?.variables?.id === payload.id;
          });

        for (const mutationToRemove of mutationsToRemove) {
          queryClient.getMutationCache().remove(mutationToRemove);
        }
      }

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
