import { queryOptions, useQuery } from '@tanstack/react-query'
import { getPollingStationsLocationLevels } from '@/services/api/polling-stations/get-polling-stations-location-levels.api'
import { PollingStationsFilters } from '@/types/polling-station'

export const pollingStationsKeys = {
  all: (electionRoundId: string) =>
    ['polling-stations', electionRoundId] as const,
  levels: (electionRoundId: string) =>
    [...pollingStationsKeys.all(electionRoundId), 'levels'] as const,
  lists: (electionRoundId: string) =>
    [...pollingStationsKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, filter: PollingStationsFilters) =>
    [...pollingStationsKeys.lists(electionRoundId), filter] as const,
  details: (electionRoundId: string) =>
    [...pollingStationsKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) =>
    [...pollingStationsKeys.details(electionRoundId), id] as const,
}

export const pollingStationsLocationLevelsQueryOptions = (
  electionRoundId: string | undefined
) =>
  queryOptions({
    queryKey: pollingStationsKeys.levels(electionRoundId as string),
    queryFn: async () =>
      await getPollingStationsLocationLevels(electionRoundId as string),
    enabled: !!electionRoundId,
  })

export function usePollingStationsLocationLevels(
  electionRoundId: string | undefined
) {
  return useQuery(pollingStationsLocationLevelsQueryOptions(electionRoundId))
}
