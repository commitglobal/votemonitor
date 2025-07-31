import { listMonitoredElections as listMonitoringElectionsAPI } from "@/services/api/monitoring-elections/list.api";
import type { ElectionsSearch } from "@/types/election";
import { queryOptions } from "@tanstack/react-query";

export const monitoredElectionsKeys = {
  all: ["monitoring-elections"] as const,
  list: (search: ElectionsSearch) =>
    [...monitoredElectionsKeys.all, { ...search }] as const,
  details: () => [...monitoredElectionsKeys.all, "detail"] as const,
  detail: (id: string) => [...monitoredElectionsKeys.details(), id] as const,
};

const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const listMonitoringElections = (search: ElectionsSearch) =>
  queryOptions({
    queryKey: monitoredElectionsKeys.list(search),
    queryFn: async () => await listMonitoringElectionsAPI(search),
    staleTime: STALE_TIME,
    refetchOnWindowFocus: false,
  });
