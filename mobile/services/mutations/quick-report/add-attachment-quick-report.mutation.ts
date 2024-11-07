import { useMutation } from "@tanstack/react-query";
import { QuickReportKeys } from "../../queries/quick-reports.query";
import {
  AddAttachmentQuickReportAbortAPIPayload,
  AddAttachmentQuickReportCompleteAPIPayload,
  AddAttachmentQuickReportStartAPIPayload,
  addAttachmentQuickReportMultipartAbort,
  addAttachmentQuickReportMultipartComplete,
  addAttachmentQuickReportMultipartStart,
} from "../../api/quick-report/add-attachment-quick-report.api";
import { MUTATION_SCOPE_DO_NOT_HYDRATE } from "../../../common/constants";
import * as Sentry from "@sentry/react-native";

// Multipart Upload - Start
export const useUploadAttachmentQuickReportMutation = () => {
  return useMutation({
    mutationKey: QuickReportKeys.addAttachment(),
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationFn: (payload: AddAttachmentQuickReportStartAPIPayload) =>
      addAttachmentQuickReportMultipartStart(payload),
    onError: (err, _variables, _context) => {
      Sentry.captureException(err, { data: { _variables, _context } });
    },
    onSettled: (_data, _err, _variables) => {},
    retry: 3,
  });
};

export const useUploadAttachmentQuickReportCompleteMutation = () => {
  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: QuickReportKeys.addAttachmentComplete(),
    mutationFn: (payload: AddAttachmentQuickReportCompleteAPIPayload) =>
      addAttachmentQuickReportMultipartComplete(payload),
    retry: 3,
  });
};

export const useUploadAttachmentQuickReportAbortMutation = () => {
  return useMutation({
    scope: {
      id: MUTATION_SCOPE_DO_NOT_HYDRATE,
    },
    mutationKey: QuickReportKeys.addAttachmentAbort(),
    mutationFn: (payload: AddAttachmentQuickReportAbortAPIPayload) =>
      addAttachmentQuickReportMultipartAbort(payload),
    retry: 3,
  });
};
