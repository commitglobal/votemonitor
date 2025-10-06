import API from "@/services/api";
import type { QuickReportFollowUpStatus } from "@/types/quick-reports";

export const updateQuickReportFollowUpStatus = async (
  electionRoundId: string,
  quickReportId: string,
  followUpStatus: QuickReportFollowUpStatus
) => {
  return API.put<void>(
    `/election-rounds/${electionRoundId}/quick-reports/${quickReportId}:status`,
    {
      followUpStatus,
    }
  );
};
