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

export const formDetailsQueryOptions = (electionRoundId: string, formId: string) => {
    return queryOptions({
        queryKey: formsKeys.detail(formId),
        queryFn: async () => {
            const response = await authApi.get<FormFull>(`/election-rounds/${electionRoundId}/forms/${formId}`);

            if (response.status !== 200) {
                throw new Error('Failed to fetch form');
            }

            return response.data;
        },
        enabled: !!electionRoundId
    });
}

export function formDetails(electionRoundId: string, formId: string): UseQueryResult<FormFull, Error> {
    return useQuery(formDetailsQueryOptions(electionRoundId, formId));
}

export function useForms(electionRoundId: string, params: DataTableParameters): UseQueryResult<PageResponse<FormBase>, Error> {
    return useQuery({
        queryKey: formsKeys.list(params),
        queryFn: async () => {
            const response = await authApi.get<PageResponse<FormBase>>(`/election-rounds/${electionRoundId}/forms`, {
                params: {
                    PageNumber: params.pageNumber,
                    PageSize: params.pageSize,
                    SortColumnName: params.sortColumnName,
                    SortOrder: params.sortOrder,
                },
            });

            if (response.status !== 200) {
                throw new Error('Failed to fetch forms');
            }

            return response.data;
        },
        enabled: !!electionRoundId
    });
}