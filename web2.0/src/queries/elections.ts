import { getById } from "@/services/api/elections/get.api";
import { listElections } from "@/services/api/elections/list.api";
import type { ElectionsSearch } from "@/types/election";
import { queryOptions, useQuery } from "@tanstack/react-query";

export const electionsKeys = {
  all: ["elections"] as const,
  list: (search: ElectionsSearch) =>
    [...electionsKeys.all, { ...search }] as const,
  details: () => [...electionsKeys.all, "detail"] as const,
  detail: (id: string) => [...electionsKeys.details(), id] as const,
};

const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const listElectionsQueryOptions = (search: ElectionsSearch) =>
  queryOptions({
    queryKey: electionsKeys.list(search),
    queryFn: async () => await listElections(search),
    staleTime: STALE_TIME,
    refetchOnWindowFocus: false,
  });

export const electionRoundDetailsQueryOptions = (electionRoundId: string) =>
  queryOptions({
    queryKey: electionsKeys.detail(electionRoundId),
    queryFn: async () => await getById(electionRoundId),
    staleTime: STALE_TIME,
    refetchOnWindowFocus: false,
  });

export const useElectionRoundDetails = (electionRoundId: string) =>
  useQuery(
    queryOptions({
      queryKey: electionsKeys.detail(electionRoundId),
      queryFn: async () => await getById(electionRoundId),
      staleTime: STALE_TIME,
      refetchOnWindowFocus: false,
    })
  );
