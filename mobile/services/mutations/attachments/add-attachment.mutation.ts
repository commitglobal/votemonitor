import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  AddAttachmentAPIPayload,
  AddAttachmentAPIResponse,
  addAttachment,
} from "../../api/add-attachment.api";
import { AttachmentApiResponse } from "../../api/get-attachments.api";
import { AttachmentsKeys } from "../../queries/attachments.query";

export const addAttachmentMutation = (scopeId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: async (payload: AddAttachmentAPIPayload): Promise<AddAttachmentAPIResponse> => {
      return addAttachment(payload);
    },
    onMutate: async (payload: AddAttachmentAPIPayload) => {
      const attachmentsQK = AttachmentsKeys.attachments(
        payload.electionRoundId,
        payload.pollingStationId,
        payload.formId,
      );

      await queryClient.cancelQueries({ queryKey: attachmentsQK });

      const previousData = queryClient.getQueryData<AttachmentApiResponse[]>(attachmentsQK);

      queryClient.setQueryData<AttachmentApiResponse[]>(attachmentsQK, [
        ...(previousData || []),
        {
          id: payload.id,
          electionRoundId: payload.electionRoundId,
          pollingStationId: payload.pollingStationId,
          formId: payload.formId,
          questionId: payload.questionId,
          fileName: `${payload.fileMetadata.name}`,
          mimeType: payload.fileMetadata.type,
          presignedUrl: payload.fileMetadata.uri, // TODO @radulescuandrew is this working to display the media?
          urlValidityInSeconds: 3600,
          isNotSynched: true,
        },
      ]);

      return { previousData, attachmentsQK };
    },
    onError: (err, payload, context) => {
      console.log(err);
      const attachmentsQK = AttachmentsKeys.attachments(
        payload.electionRoundId,
        payload.pollingStationId,
        payload.formId,
      );
      queryClient.setQueryData(attachmentsQK, context?.previousData);
    },
    onSettled: (_data, _err, variables) => {
      return queryClient.invalidateQueries({
        queryKey: AttachmentsKeys.attachments(
          variables.electionRoundId,
          variables.pollingStationId,
          variables.formId,
        ),
      });
    },
  });
};
