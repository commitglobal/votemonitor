import { useMutation } from "@tanstack/react-query";
import { QuickReportKeys } from "../../queries/quick-reports.query";

export const addAttachmentQuickReportMutation = () => {
  return useMutation({
    mutationKey: QuickReportKeys.addAttachment(),
    onError: (err, _payload, _context) => {
      console.log(err);
    },
    onSettled: (_data, _err, _variables) => {},
  });
};
