import { updateQuickReportFollowUpStatus } from "@/services/api/quick-reports/update-status.api";
import type { QuickReportFollowUpStatus } from "@/types/quick-reports";
import { useMutation } from "@tanstack/react-query";

export const useUpdateQuickReportFollowUpStatusMutation = () =>
  useMutation({
    mutationFn: async ({
      electionRoundId,
      quickReportId,
      followUpStatus,
    }: {
      electionRoundId: string;
      quickReportId: string;
      followUpStatus: QuickReportFollowUpStatus;
    }) =>
      await updateQuickReportFollowUpStatus(
        electionRoundId,
        quickReportId,
        followUpStatus
      ),
  });
