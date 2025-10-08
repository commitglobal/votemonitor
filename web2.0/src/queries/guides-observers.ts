import { queryOptions, useQuery } from "@tanstack/react-query";
import type {GuidesObserversSearch} from "@/types/guides-observer.ts";
import {listGuidesObservers} from "@/services/api/guides-observers/list.api.ts";

export const guidesObserversKeys = {
    all: (electionRoundId: string) =>
        ["guides-observers", electionRoundId] as const,
    lists: (electionRoundId: string) =>
        [...guidesObserversKeys.all(electionRoundId), "list"] as const,
    list: (electionRoundId: string, search: GuidesObserversSearch) =>
        [...guidesObserversKeys.lists(electionRoundId), { ...search }] as const,
    details: (electionRoundId: string) =>
        [...guidesObserversKeys.all(electionRoundId), "detail"] as const,
    detail: (electionRoundId: string, id: string) =>
        [...guidesObserversKeys.details(electionRoundId), id] as const,
};

const STALE_TIME = 1000 * 60 * 15; // 15 minutes

export const listGuidesObserversQueryOptions = (
    electionRoundId: string,
    search: GuidesObserversSearch
) =>
    queryOptions({
        queryKey: guidesObserversKeys.list(electionRoundId, search),
        queryFn: async () => await listGuidesObservers(electionRoundId, search),
        enabled: !!electionRoundId,
        staleTime: STALE_TIME,
        refetchOnWindowFocus: false,
    });

export const useListGuidesObservers = (
    electionRoundId: string,
    search: GuidesObserversSearch
) => useQuery(listGuidesObserversQueryOptions(electionRoundId, search));
