import { queryOptions } from "@tanstack/react-query";
import { authApi } from "./auth-api";
import { MonitoringObserver } from "@/features/monitoring-observers/models/monitoring-observer";

export const monitoringObserverDetailsQueryOptions = (monitoringObserverId: string) =>
    queryOptions({
      queryKey: ['monitoring-observers', { monitoringObserverId }],
      queryFn: async () => {
        const electionRoundId: string | null = localStorage.getItem('electionRoundId');
  
        const response = await authApi.get<MonitoringObserver>(
          `/election-rounds/${electionRoundId}/monitoring-observers/${monitoringObserverId}`
        );
  
        if (response.status !== 200) {
          throw new Error('Failed to fetch monitoring observer details');
        }
  
        return response.data;
      },
    });