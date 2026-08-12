import { queryOptions, useSuspenseQuery } from '@tanstack/react-query'
import { listGuidesObservers } from '@/services/api/guides-observers/list.api'
import type { GuidesObserverModel } from '@/types/guides-observer'

export const guidesObserversKeys = {
  all: (electionRoundId: string) =>
    ['guides-observers', electionRoundId] as const,
  lists: (electionRoundId: string) =>
    [...guidesObserversKeys.all(electionRoundId), 'list'] as const,
}

const STALE_TIME = 1000 * 60 * 15 // 15 minutes

/**
 * The key deliberately holds nothing but the election round id: the endpoint
 * returns the whole collection and ignores search params, so folding them into
 * the key would refetch the exact same payload on every keystroke.
 */
export const listGuidesObserversQueryOptions = <
  TResult = GuidesObserverModel[],
>(
  electionRoundId: string,
  select?: (data: GuidesObserverModel[]) => TResult
) =>
  queryOptions({
    queryKey: guidesObserversKeys.lists(electionRoundId),
    queryFn: async () => await listGuidesObservers(electionRoundId),
    staleTime: STALE_TIME,
    select,
  })

export const useSuspenseListGuidesObservers = <TResult = GuidesObserverModel[]>(
  electionRoundId: string,
  select?: (data: GuidesObserverModel[]) => TResult
) => useSuspenseQuery(listGuidesObserversQueryOptions(electionRoundId, select))
