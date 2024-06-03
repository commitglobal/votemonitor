import { useMutation } from "@tanstack/react-query";
import { QuickReportKeys } from "../../queries/quick-reports.query";
import {
  AddAttachmentQuickReportStartAPIPayload,
  addAttachmentQuickReportMultipartStart,
} from "../../api/quick-report/add-attachment-quick-report.api";

// Multipart Upload - Start
export const useUploadAttachmentQuickReportMutation = (scopeId: string) => {
  return useMutation({
    mutationKey: QuickReportKeys.addAttachment(),
    scope: {
      id: scopeId,
    },
    mutationFn: (payload: AddAttachmentQuickReportStartAPIPayload) =>
      addAttachmentQuickReportMultipartStart(payload),
    onError: (err, _variables, _context) => {
      console.log(err);
    },
    onSettled: (_data, _err, _variables) => {},
  });
};
