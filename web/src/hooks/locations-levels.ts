import API from '@/services/api';
import type { DataTableParameters, LevelNode } from '@/common/types';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
const STALE_TIME = 1000 * 60 * 5; // five minutes

export const locationsKeys = {
  all: (electionRoundId: string) => ['locations', electionRoundId] as const,
  levels: (electionRoundId: string) => [...locationsKeys.all(electionRoundId), 'levels'] as const,
  lists: (electionRoundId: string) => [...locationsKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...locationsKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...locationsKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...locationsKeys.details(electionRoundId), id] as const,
};

type LocationsLevelsResponse = { nodes: LevelNode[] };

type UseLocationsLevelsResult = UseQueryResult<Record<string, LevelNode[]>, Error>;

export function useLocationsLevels(electionRoundId: string): UseLocationsLevelsResult {
  return useQuery({
    queryKey: locationsKeys.levels(electionRoundId),
    queryFn: async () => {
      const response = await API.get<LocationsLevelsResponse>(`/election-rounds/${electionRoundId}/locations:fetchAll`);

      return response.data.nodes.reduce<Record<string, LevelNode[]>>(
        (group, node) => ({ ...group, [node.depth]: [...(group[node.depth] ?? []), node] }),
        {}
      );
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
}
