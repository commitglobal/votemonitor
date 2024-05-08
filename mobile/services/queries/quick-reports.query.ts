import { skipToken, useQuery } from "@tanstack/react-query";
import { getQuickReports } from "../api/quick-report/get-quick-reports.api";

export const QuickReportKeys = {
  all: ["quick-reports"] as const,
  byElectionRound: (electionRoundId: string | undefined) => [
    ...QuickReportKeys.all,
    "electionRoundId",
    electionRoundId,
  ],
  add: () => [...QuickReportKeys.all, "add"] as const,
  addAttachment: () => [...QuickReportKeys.all, "addAttachment"],
};

export const useQuickReports = (electionRoundId: string | undefined) => {
  return useQuery({
    queryKey: QuickReportKeys.byElectionRound(electionRoundId),
    queryFn: electionRoundId ? () => getQuickReports(electionRoundId) : skipToken,
  });
};
