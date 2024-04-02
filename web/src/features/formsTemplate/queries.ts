import { authApi } from "@/common/auth-api";
import { DataTableParameters, PageResponse } from "@/common/types";
import { UseQueryResult, queryOptions, useQuery } from "@tanstack/react-query";
import { FormTemplateBase, FormTemplateFull } from "./models/formTemplate";

export const formTemplatesKeys = {
    all: ['form-templates'] as const,
    lists: () => [...formTemplatesKeys.all, 'list'] as const,
    list: (params: DataTableParameters) => [...formTemplatesKeys.lists(), { ...params }] as const,
    details: () => [...formTemplatesKeys.all, 'detail'] as const,
    detail: (id: string) => [...formTemplatesKeys.details(), id] as const,
}

export const formTemplateDetailsQueryOptions = (formTemplateId: string) =>
    queryOptions({
        queryKey: formTemplatesKeys.detail(formTemplateId),
        queryFn: async () => {
            const response = await authApi.get<FormTemplateFull>(`/form-templates/${formTemplateId}`);

            if (response.status !== 200) {
                throw new Error('Failed to fetch form template');
            }

            return response.data;
        },
    });


export function formTemplateDetails(formTemplateId: string): UseQueryResult<FormTemplateFull, Error> {
    return useQuery(formTemplateDetailsQueryOptions(formTemplateId));
}

export function useFormTemplates(p: DataTableParameters): UseQueryResult<PageResponse<FormTemplateBase>, Error> {
    return useQuery({
        queryKey: formTemplatesKeys.list(p),
        queryFn: async () => {
            const response = await authApi.get<PageResponse<FormTemplateBase>>('/form-templates', {
                params: {
                    PageNumber: p.pageNumber,
                    PageSize: p.pageSize,
                    SortColumnName: p.sortColumnName,
                    SortOrder: p.sortOrder,
                },
            });

            if (response.status !== 200) {
                throw new Error('Failed to fetch form templates');
            }

            return response.data;
        },
    });
}