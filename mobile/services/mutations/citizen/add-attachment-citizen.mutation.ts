import { useMutation } from "@tanstack/react-query";
import {
  AddAttachmentCitizenAbortAPIPayload,
  AddAttachmentCitizenCompleteAPIPayload,
  addAttachmentCitizenMultipartAbort,
  addAttachmentCitizenMultipartComplete,
  addAttachmentCitizenMultipartStart,
  AddAttachmentCitizenStartAPIPayload,
} from "../../api/citizen/attachments.api";
import { citizenQueryKeys } from "../../queries/citizen.query";
import * as Sentry from "@sentry/react-native";
import { MUTATION_SCOPE_DO_NOT_HYDRATE } from "../../../common/constants";

// Multipart Upload - Start
export const useUploadAttachmentCitizenMutation = () => {
  return useMutation({
    mutationKey: citizenQueryKeys.attachments(),
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationFn: (payload: AddAttachmentCitizenStartAPIPayload) =>
      addAttachmentCitizenMultipartStart(payload),
    onError: (err, _variables, _context) => {
      Sentry.captureException(err, { data: { _variables, _context } });
    },
    onSettled: (_data, _err, _variables) => {},
    retry: 3,
  });
};

export const useUploadAttachmentCitizenCompleteMutation = () => {
  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: citizenQueryKeys.attachments(),
    mutationFn: (payload: AddAttachmentCitizenCompleteAPIPayload) =>
      addAttachmentCitizenMultipartComplete(payload),
    retry: 3,
  });
};

export const useUploadAttachmentCitizenAbortMutation = () => {
  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: citizenQueryKeys.attachments(),
    mutationFn: (payload: AddAttachmentCitizenAbortAPIPayload) =>
      addAttachmentCitizenMultipartAbort(payload),
    retry: 3,
  });
};
