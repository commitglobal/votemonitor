import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import { authApi } from '@/common/auth-api';

type MonitoringObserversTagsResponse = { tags: string[] };

type UseMonitoringObserversTagsResult = UseQueryResult<MonitoringObserversTagsResponse, Error>;

export function useMonitoringObserversTags(): UseMonitoringObserversTagsResult {
  return useQuery({
    queryKey: ['tags'],
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');
      const monitoringNgoId: string | null = localStorage.getItem('monitoringNgoId');

      const response = await authApi.get<MonitoringObserversTagsResponse>(
        `/election-rounds/${electionRoundId}/monitoring-ngos/${monitoringNgoId}/monitoring-observers:tags`
      );

      return response.data.tags.length > 0 ? response.data : { tags: ['one', 'two'] };
    },
  });
}
