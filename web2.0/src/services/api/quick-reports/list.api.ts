import { buildURLSearchParams } from "@/lib/utils";
import API from "@/services/api";
import type { PageResponse } from "@/types/common";
import type {
  QuickReportModel,
  QuickReportsSearch,
} from "@/types/quick-reports";

export const listQuickReports = async (
  electionRoundId: string,
  search: QuickReportsSearch
): Promise<PageResponse<QuickReportModel>> => {
  return API.get(`/election-rounds/${electionRoundId}/quick-reports`, {
    params: buildURLSearchParams(search),
  }).then((res) => res.data);
};
