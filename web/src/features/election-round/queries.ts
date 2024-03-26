import { authApi } from "@/common/auth-api";
import { DataTableParameters, PageResponse } from "@/common/types";
import { UseQueryResult, useQuery } from "@tanstack/react-query";
import { ElectionRound } from "./models/ElectionRound";


export const electionRoundKeys = {
    all: ['electionRounds'] as const,
    lists: () => [...electionRoundKeys.all, 'list'] as const,
    list: (params: DataTableParameters) => [...electionRoundKeys.lists(), { ...params }] as const,
    details: () => [...electionRoundKeys.all, 'detail'] as const,
    detail: (id: string) => [...electionRoundKeys.details(), id] as const,
}

export function useElectionRounds(p: DataTableParameters): UseQueryResult<PageResponse<ElectionRound>, Error> {
    return useQuery({
        queryKey: electionRoundKeys.list(p),
        queryFn: async () => {
            const response = await authApi.get<PageResponse<ElectionRound>>('/election-rounds', {
                params: {
                    PageNumber: p.pageNumber,
                    PageSize: p.pageSize,
                    SortColumnName: p.sortColumnName,
                    SortOrder: p.sortOrder,
                },
            });

            if (response.status !== 200) {
                throw new Error('Failed to fetch electionRounds');
            }

            return response.data;
        },
    });
}
