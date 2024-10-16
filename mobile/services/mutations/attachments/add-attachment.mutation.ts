import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  AddAttachmentMultipartStartAPIResponse,
  AddAttachmentStartAPIPayload,
  addAttachmentMultipartStart,
} from "../../api/add-attachment.api";
import { AttachmentsKeys } from "../../queries/attachments.query";
import * as Sentry from "@sentry/react-native";

// Multipart Upload - Start
export const useUploadAttachmentMutation = (scopeId: string) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: AttachmentsKeys.addAttachmentMutation(),
    scope: {
      id: scopeId,
    },
    mutationFn: (
      payload: AddAttachmentStartAPIPayload,
    ): Promise<AddAttachmentMultipartStartAPIResponse> => addAttachmentMultipartStart(payload),
    onError: (err, payload, context) => {
      Sentry.captureException(err, { data: { payload, context } });
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
