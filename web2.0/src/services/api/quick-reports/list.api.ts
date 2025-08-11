import { buildURLSearchParams } from "@/lib/utils";
import API from "@/services/api";
import type { PageResponse } from "@/types/common";
import type { ElectionsSearch } from "@/types/election";
import type { QuickReportModel } from "@/types/quick-reports";

export const listQuickReports = async (
  electionRoundId: string,
  search: ElectionsSearch
): Promise<PageResponse<QuickReportModel>> => {
  return API.get(`/election-rounds/${electionRoundId}/quick-reports`, {
    params: buildURLSearchParams(search),
  }).then((res) => res.data);
};
