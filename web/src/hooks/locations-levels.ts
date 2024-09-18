import { authApi } from '@/common/auth-api';
import type { LevelNode } from '@/common/types';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';

type LocationsLevelsResponse = { nodes: LevelNode[] };

type UseLocationsLevelsResult = UseQueryResult<Record<string, LevelNode[]>, Error>;

export function useLocationsLevels(electionRoundId: string): UseLocationsLevelsResult {
  return useQuery({
    queryKey: ['locations', 'levels'],
    queryFn: async () => {

      const response = await authApi.get<LocationsLevelsResponse>(
        `/election-rounds/${electionRoundId}/locations:fetchLevels`
      );

      return response.data.nodes.reduce<Record<string, LevelNode[]>>(
        (group, node) => ({ ...group, [node.depth]: [...(group[node.depth] ?? []), node] }),
        {}
      );
    },
    enabled: !!electionRoundId
  });
}
