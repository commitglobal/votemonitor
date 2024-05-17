import { authApi } from "@/common/auth-api";
import { Tag } from "@/components/tag/tag-input";
import { UseQueryResult, useQuery } from "@tanstack/react-query";
import { v4 as uuid } from 'uuid';

export function useTags(): UseQueryResult<Tag[], Error> {
   return useQuery({
        queryKey: ['tags'],
        queryFn: async () => {
            const electionRoundId: string | null = localStorage.getItem('electionRoundId');
            const monitoringNgoId: string | null = localStorage.getItem('monitoringNgoId');

            const response = await authApi.get<{ tags: string[] }>(
                `/election-rounds/${electionRoundId}/monitoring-ngos/${monitoringNgoId}/monitoring-observers:tags`
            );

            if (response.status !== 200) {
                throw new Error('Failed to fetch monitoring observers');
            }
            return response.data?.tags.map((tag) => ({ id: uuid(), text: tag }));
        },
    });
}