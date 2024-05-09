import { useMutation, useQueryClient } from "@tanstack/react-query";
import { pollingStationsKeys } from "../queries.service";
import {
  AddAttachmentAPIPayload,
  AddAttachmentAPIResponse,
  addAttachment,
} from "../api/add-attachment.api";
import { performanceLog } from "../../helpers/misc";
import { AttachmentApiResponse } from "../api/get-attachments.api";

export const addAttachmentMutation = (scopeId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: pollingStationsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: async (payload: AddAttachmentAPIPayload): Promise<AddAttachmentAPIResponse> => {
      return performanceLog(() => addAttachment(payload));
    },
    onMutate: async (payload: AddAttachmentAPIPayload) => {
      const attachmentsQK = pollingStationsKeys.attachments(
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
          fileName: `🛜 ${payload.fileMetadata.name}`,
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
      const attachmentsQK = pollingStationsKeys.attachments(
        payload.electionRoundId,
        payload.pollingStationId,
        payload.formId,
      );
      queryClient.setQueryData(attachmentsQK, context?.previousData);
    },
    onSettled: (_data, _err, variables) => {
      return queryClient.invalidateQueries({
        queryKey: pollingStationsKeys.attachments(
          variables.electionRoundId,
          variables.pollingStationId,
          variables.formId,
        ),
      });
    },
  });
};
