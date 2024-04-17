import { authApi } from "@/common/auth-api";
import { UseQueryResult, useQuery } from "@tanstack/react-query";
import { Language } from "./language";

export const languagesQuery = {
    queryKey: ['languages'],
    queryFn: async () => {
        const response = await authApi.get<Language[]>('/languages');

        if (response.status !== 200) {
            throw new Error('Failed to fetch countries');
        }

        return response.data;
    },
    refetchOnMount: false,
    staleTime: Number.POSITIVE_INFINITY
}; 

export function useLanguages(): UseQueryResult<Language[], Error> {
    return useQuery(languagesQuery);
}