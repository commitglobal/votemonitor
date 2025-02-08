import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams } from '@/lib/utils';
import { queryClient } from '@/main';
import { UseQueryResult, queryOptions, useQuery } from '@tanstack/react-query';
import { FormTemplateBase, FormTemplateFull } from './models';
const STALE_TIME = 1000 * 60 * 5; // five minutes

export const formTemlatesKeys = {
  all: () => ['form-templates' ] as const,
  lists: () => [...formTemlatesKeys.all(), 'list'] as const,
  list: (params: DataTableParameters) =>
    [...formTemlatesKeys.lists(), { ...params }] as const,
  details: () => [...formTemlatesKeys.all(), 'detail'] as const,
  detail: (id: string) => [...formTemlatesKeys.details(), id] as const,
  baseDetails: ( id: string) => [...formTemlatesKeys.details(), 'base', id] as const,
};

export const formTemplateDetailsQueryOptions = (formTemplateId: string) => {
  return queryOptions({
    queryKey: formTemlatesKeys.detail(formTemplateId),
    queryFn: async () => {
      const response = await authApi.get<FormTemplateFull>(`/form-templates/${formTemplateId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch form');
      }

      return response.data;
    },
    enabled: !!formTemplateId,
  });
};

export function useformTemplateDetails(formTemplateId: string): UseQueryResult<FormTemplateFull, Error> {
  return useQuery(formTemplateDetailsQueryOptions( formTemplateId));
}

export function useFormTemplates(
  queryParams: DataTableParameters
): UseQueryResult<PageResponse<FormTemplateBase>, Error> {

  return useQuery({
    queryKey: formTemlatesKeys.list(queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };

      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<PageResponse<FormTemplateBase>>(`/form-templates`, {
        params: searchParams,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch form templates');
      }

      response.data.items.forEach((formTemplate) => {
        queryClient.setQueryData(formTemlatesKeys.baseDetails(formTemplate.id), formTemplate);
      });

      return response.data;
    },
    staleTime: STALE_TIME,
  });
}
