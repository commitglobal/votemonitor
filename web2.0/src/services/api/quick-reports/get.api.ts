import API from "@/services/api";
import type { QuickReportModel } from "@/types/quick-reports";

export const getById = async (
  electionRoundId: string,
  quickReportId: string
): Promise<QuickReportModel> => {
  return API.get(
    `/election-rounds/${electionRoundId}/quick-reports/${quickReportId}`
  ).then((res) => res.data);
};
