import { authApi } from "@/common/auth-api";
import { DataTableParameters, PageResponse } from "@/common/types";
import { UseQueryResult, queryOptions, useQuery } from "@tanstack/react-query";
import { FormBase, FormFull } from "./models/form";

export const formsKeys = {
    all: ['forms'] as const,
    lists: () => [...formsKeys.all, 'list'] as const,
    list: (params: DataTableParameters) => [...formsKeys.lists(), { ...params }] as const,
    details: () => [...formsKeys.all, 'detail'] as const,
    detail: (id: string) => [...formsKeys.details(), id] as const,
}

export const formDetailsQueryOptions = (formId: string) =>
    queryOptions({
        queryKey: formsKeys.detail(formId),
        queryFn: async () => {
            const electionRoundId: string | null = localStorage.getItem('electionRoundId');

            const response = await authApi.get<FormFull>(`/election-rounds/${electionRoundId}/forms/${formId}`);

            if (response.status !== 200) {
                throw new Error('Failed to fetch form');
            }

            return response.data;
        },
    });


export function formDetails(formId: string): UseQueryResult<FormFull, Error> {
    return useQuery(formDetailsQueryOptions(formId));
}

export function useForms(p: DataTableParameters): UseQueryResult<PageResponse<FormBase>, Error> {
    return useQuery({
        queryKey: formsKeys.list(p),
        queryFn: async () => {
            const electionRoundId: string | null = localStorage.getItem('electionRoundId');

            const response = await authApi.get<PageResponse<FormBase>>(`/election-rounds/${electionRoundId}/forms`, {
                params: {
                    PageNumber: p.pageNumber,
                    PageSize: p.pageSize,
                    SortColumnName: p.sortColumnName,
                    SortOrder: p.sortOrder,
                },
            });

            if (response.status !== 200) {
                throw new Error('Failed to fetch forms');
            }

            return response.data;
        },
    });
}