import { useMutation } from "@tanstack/react-query";
import {
  CitizenReportCopyAPIPayload,
  putCitizenReportCopy,
} from "../../api/citizen/put-citizen-report-copy";

export const usePutCitizenReportCopyMutation = () => {
  return useMutation({
    mutationKey: ["putCitizenReportCopy"],
    mutationFn: (data: CitizenReportCopyAPIPayload) => putCitizenReportCopy(data),
  });
};
