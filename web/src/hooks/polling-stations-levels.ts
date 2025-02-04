import { authApi } from '@/common/auth-api';
import type { DataTableParameters, LevelNode } from '@/common/types';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';

export const pollingStationsKeys = {
  all: (electionRoundId: string) => ['polling-stations', electionRoundId] as const,
  levels: (electionRoundId: string) => [...pollingStationsKeys.all(electionRoundId), 'levels'] as const,
  lists: (electionRoundId: string) => [...pollingStationsKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...pollingStationsKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...pollingStationsKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...pollingStationsKeys.details(electionRoundId), id] as const,
};

const STALE_TIME = 1000 * 60 * 15; // fifteen minutes

type PollingStationsLocationLevelsResponse = { nodes: LevelNode[] };

type UsePollingStationsLocationLevelsResult = UseQueryResult<Record<string, LevelNode[]>, Error>;

export function usePollingStationsLocationLevels(electionRoundId: string): UsePollingStationsLocationLevelsResult {
  return useQuery({
    queryKey: pollingStationsKeys.levels(electionRoundId),
    queryFn: async () => {
      const response = await authApi.get<PollingStationsLocationLevelsResponse>(
        `/election-rounds/${electionRoundId}/polling-stations:fetchLevels`
      );

      return response.data.nodes.reduce<Record<string, LevelNode[]>>(
        (group, node) => ({ ...group, [node.depth]: [...(group[node.depth] ?? []), node] }),
        {}
      );
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
}
