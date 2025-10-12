import { getById } from "@/services/api/quick-reports/get.api";
import { listQuickReports } from "@/services/api/quick-reports/list.api";
import type { DataSource, PageResponse } from "@/types/common";
import type {
  QuickReportModel,
  QuickReportsSearch,
} from "@/types/quick-reports";
import {
  queryOptions,
  useQuery,
  useSuspenseQuery,
} from "@tanstack/react-query";

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

export const listQuickReportsQueryOptions = <
  TResult = PageResponse<QuickReportModel>
>(
  electionRoundId: string,
  search: QuickReportsSearch,
  select?: (data: PageResponse<QuickReportModel>) => TResult
) =>
  queryOptions({
    queryKey: quickReportKeys.list(electionRoundId, search),
    queryFn: async () => await listQuickReports(electionRoundId, search),
    staleTime: STALE_TIME,
    select,
  });

export const useListQuickReports = <TResult = PageResponse<QuickReportModel>>(
  electionRoundId: string,
  search: QuickReportsSearch,
  select?: (data: PageResponse<QuickReportModel>) => TResult
) => useQuery(listQuickReportsQueryOptions(electionRoundId, search, select));

export function getQuickReportDetailsQueryOptions<TResult = QuickReportModel>(
  electionRoundId: string,
  quickReportId: string,
  select?: (data: QuickReportModel) => TResult
) {
  return queryOptions({
    queryKey: quickReportKeys.detail(electionRoundId, quickReportId),
    queryFn: async () => await getById(electionRoundId, quickReportId),
    staleTime: STALE_TIME,
    select,
  });
}

export const useSuspenseGetQuickReportDetails = <TResult = QuickReportModel>(
  electionRoundId: string,
  quickReportId: string,
  select?: (data: QuickReportModel) => TResult
) =>
  useSuspenseQuery(
    getQuickReportDetailsQueryOptions(electionRoundId, quickReportId, select)
  );
