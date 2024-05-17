import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import { authApi } from './auth-api';
import type { LevelNode } from './types';

type PollingStationsResponse = { nodes: LevelNode[] };

type UsePollingStationsResult = UseQueryResult<Record<string, LevelNode[]>, Error>;

export function usePollingStations(): UsePollingStationsResult {
  return useQuery({
    queryKey: ['polling-stations', 'levels'],
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<PollingStationsResponse>(
        `/election-rounds/${electionRoundId}/polling-stations:fetchLevels`
      );

      return response.data.nodes.reduce<Record<string, LevelNode[]>>(
        (group, node) => ({ ...group, [node.depth]: [...(group[node.depth] ?? []), node] }),
        {}
      );
    },
  });
}
