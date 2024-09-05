import { MonitoringObserver } from "@/features/monitoring-observers/models/monitoring-observer";
import { queryOptions } from "@tanstack/react-query";
import { authApi } from "./auth-api";

export const monitoringObserverDetailsQueryOptions = (electionRoundId: string, monitoringObserverId: string) => {
  return queryOptions({
    queryKey: ['monitoring-observers', { electionRoundId, monitoringObserverId }],
    queryFn: async () => {
      const response = await authApi.get<MonitoringObserver>(
        `/election-rounds/${electionRoundId}/monitoring-observers/${monitoringObserverId}`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch monitoring observer details');
      }

      return response.data;
    },
    enabled: !!electionRoundId
  });
}