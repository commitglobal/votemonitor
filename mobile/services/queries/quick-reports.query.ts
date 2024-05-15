import { skipToken, useQuery } from "@tanstack/react-query";
import {
  getQuickReports,
  QuickReportsAPIResponse,
} from "../api/quick-report/get-quick-reports.api";
import { useCallback } from "react";

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

export const useQuickReports = <TResult = QuickReportsAPIResponse[]>(
  electionRoundId?: string,
  select?: (data: QuickReportsAPIResponse[]) => TResult,
) => {
  return useQuery({
    queryKey: QuickReportKeys.byElectionRound(electionRoundId),
    queryFn: electionRoundId ? () => getQuickReports(electionRoundId) : skipToken,
    select,
    initialData: [],
  });
};

export const useQuickReportById = (electionRoundId: string | undefined, quickReportId: string) => {
  return useQuickReports(
    electionRoundId,
    useCallback(
      (data: QuickReportsAPIResponse[]) => {
        const selectedQuickReport = data.find((quickReport) => quickReport.id === quickReportId);
        return selectedQuickReport;
      },
      [electionRoundId, quickReportId],
    ),
  );
};