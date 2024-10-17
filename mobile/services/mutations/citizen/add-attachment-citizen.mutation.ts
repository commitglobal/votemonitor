import { useMutation } from "@tanstack/react-query";
import {
  addAttachmentCitizenMultipartStart,
  AddAttachmentCitizenStartAPIPayload,
} from "../../api/citizen/attachments.api";
import { citizenQueryKeys } from "../../queries/citizen.query";
import * as Sentry from "@sentry/react-native";

// Multipart Upload - Start
export const useUploadAttachmentCitizenMutation = () => {
  return useMutation({
    mutationKey: citizenQueryKeys.attachments(),
    mutationFn: (payload: AddAttachmentCitizenStartAPIPayload) =>
      addAttachmentCitizenMultipartStart(payload),
    onError: (err, _variables, _context) => {
      Sentry.captureException(err, { data: { _variables, _context } });
    },
    onSettled: (_data, _err, _variables) => {},
  });
};
