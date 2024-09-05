import { authApi } from "@/common/auth-api";
import { UseQueryResult, useQuery } from "@tanstack/react-query";

export function useMonitoringObserversTags(electionRoundId: string): UseQueryResult<string[], Error> {
    return useQuery({
        queryKey: ['tags', electionRoundId],
        queryFn: async () => {

            const response = await authApi.get<{ tags: string[] }>(
                `/election-rounds/${electionRoundId}/monitoring-observers:tags`
            );

            if (response.status !== 200) {
                throw new Error('Failed to fetch monitoring observers tags');
            }
            return response.data?.tags ?? [];
        },
        enabled: !!electionRoundId
    });
}
