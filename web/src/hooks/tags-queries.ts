import API from '@/services/api';
import { UseQueryResult, useQuery } from '@tanstack/react-query';
const STALE_TIME = 1000 * 60 * 5; // fifteen minutes

export function useMonitoringObserversTags(electionRoundId: string): UseQueryResult<string[], Error> {
  return useQuery({
    queryKey: ['tags', electionRoundId],
    queryFn: async () => {
      const response = await API.get<{ tags: string[] }>(
        `/election-rounds/${electionRoundId}/monitoring-observers:tags`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch monitoring observers tags');
      }
      return response.data?.tags ?? [];
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
}
