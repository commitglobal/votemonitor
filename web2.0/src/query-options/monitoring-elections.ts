import { listMonitoredElections as listMonitoringElectionsAPI } from "@/services/api/monitoring-elections/list.api";
import { queryOptions, useQuery } from "@tanstack/react-query";

export const monitoredElectionsKeys = {
  all: ["monitoring-elections"] as const,
  list: () => [...monitoredElectionsKeys.all, "list"] as const,
  details: () => [...monitoredElectionsKeys.list(), "detail"] as const,
  detail: (id: string) => [...monitoredElectionsKeys.details(), id] as const,
};

const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const listMonitoringElections = () =>
  queryOptions({
    queryKey: monitoredElectionsKeys.list(),
    queryFn: async () => await listMonitoringElectionsAPI(),
    staleTime: STALE_TIME,
    refetchOnWindowFocus: false,
  });

export const useListMonitoringElections = () =>
  useQuery(listMonitoringElections());
