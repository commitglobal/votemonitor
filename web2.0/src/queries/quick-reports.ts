import { getById } from "@/services/api/quick-reports/get.api";
import { listQuickReports } from "@/services/api/quick-reports/list.api";
import type { DataSource } from "@/types/common";
import type { QuickReportsSearch } from "@/types/quick-reports";
import { queryOptions, useQuery } from "@tanstack/react-query";

export const quickReportKeys = {
  all: (electionRoundId: string) => ["quick-reports", electionRoundId] as const,
  lists: (electionRoundId: string) =>
    [...quickReportKeys.all(electionRoundId), "list"] as const,
  list: (electionRoundId: string, params: QuickReportsSearch) =>
    [...quickReportKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) =>
    [...quickReportKeys.all(electionRoundId), "detail"] as const,
  detail: (electionRoundId: string, quickReportId: string) =>
    [...quickReportKeys.details(electionRoundId), quickReportId] as const,
  filters: (electionRoundId: string, dataSource: DataSource) =>
    [
      ...quickReportKeys.details(electionRoundId),
      dataSource,
      "filters",
    ] as const,
};

const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const quickReportsQueryOptions = (
  electionRoundId: string,
  search: QuickReportsSearch
) =>
  queryOptions({
    queryKey: quickReportKeys.list(electionRoundId, search),
    queryFn: async () => await listQuickReports(electionRoundId, search),
    staleTime: STALE_TIME,
    refetchOnWindowFocus: false,
  });

export const useQuickReports = (
  electionRoundId: string,
  search: QuickReportsSearch
) => useQuery(quickReportsQueryOptions(electionRoundId, search));

export function quickReportDetailsQueryOptions(
  electionRoundId: string,
  quickReportId: string
) {
  return queryOptions({
    queryKey: quickReportKeys.detail(electionRoundId, quickReportId),
    queryFn: async () => await getById(electionRoundId, quickReportId),
    staleTime: STALE_TIME,
    refetchOnWindowFocus: false,
  });
}
