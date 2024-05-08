import { useMutation } from "@tanstack/react-query";
import { QuickReportKeys } from "../../queries/quick-reports.query";
import {
  AddAttachmentQuickReportAPIPayload,
  addAttachmentQuickReport,
} from "../../api/quick-report/add-attachment-quick-report.api";
import { performanceLog } from "../../../helpers/misc";

export const addAttachmentQuickReportMutation = () => {
  return useMutation({
    mutationKey: QuickReportKeys.addAttachment(),
    mutationFn: async (payload: AddAttachmentQuickReportAPIPayload) => {
      return performanceLog(() => addAttachmentQuickReport(payload));
    },
    onError: (err, _variables, _context) => {
      console.log(err);
    },
    onSettled: (_data, _err, _variables) => {},
  });
};
