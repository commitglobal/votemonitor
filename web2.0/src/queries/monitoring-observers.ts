import { listMonitoringObserversTags } from "@/services/api/monitoring-observers/list-tags.api";
import { listMonitoringObservers } from "@/services/api/monitoring-observers/list.api";
import type { MonitoringObserversSearch } from "@/types/monitoring-observer";
import { queryOptions, useQuery } from "@tanstack/react-query";

export const monitoringObserversKeys = {
  all: (electionRoundId: string) =>
    ["monitoring-observers", electionRoundId] as const,
  lists: (electionRoundId: string) =>
    [...monitoringObserversKeys.all(electionRoundId), "list"] as const,
  list: (electionRoundId: string, search: MonitoringObserversSearch) =>
    [...monitoringObserversKeys.lists(electionRoundId), { ...search }] as const,
  details: (electionRoundId: string) =>
    [...monitoringObserversKeys.all(electionRoundId), "detail"] as const,
  detail: (electionRoundId: string, id: string) =>
    [...monitoringObserversKeys.details(electionRoundId), id] as const,
  tags: (electionRoundId: string) =>
    [...monitoringObserversKeys.details(electionRoundId), "tags"] as const,
};

const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const listMonitoringObserversQueryOptions = (
  electionRoundId: string,
  search: MonitoringObserversSearch
) =>
  queryOptions({
    queryKey: monitoringObserversKeys.list(electionRoundId, search),
    queryFn: async () => await listMonitoringObservers(electionRoundId, search),
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
    refetchOnWindowFocus: false,
  });

export const useListMonitoringObservers = (
  electionRoundId: string,
  search: MonitoringObserversSearch
) => useQuery(listMonitoringObserversQueryOptions(electionRoundId, search));

export const listMonitoringObserversTagsQueryOptions = (
  electionRoundId: string
) =>
  queryOptions({
    queryKey: monitoringObserversKeys.tags(electionRoundId),
    queryFn: async () => await listMonitoringObserversTags(electionRoundId),
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });

export const useListMonitoringObserversTags = (electionRoundId: string) =>
  useQuery(listMonitoringObserversTagsQueryOptions(electionRoundId));
