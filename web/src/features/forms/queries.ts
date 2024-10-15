import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import { UseQueryResult, queryOptions, useQuery } from '@tanstack/react-query';
import { FormBase, FormFull } from './models/form';
import { buildURLSearchParams } from '@/lib/utils';

export const formsKeys = {
  all: (electionRoundId: string) => ['forms', electionRoundId] as const,
  lists: (electionRoundId: string) => [...formsKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...formsKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...formsKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...formsKeys.details(electionRoundId), id] as const,
};

export const formDetailsQueryOptions = (electionRoundId: string, formId: string) => {
  return queryOptions({
    queryKey: formsKeys.detail(electionRoundId, formId),
    queryFn: async () => {
      const response = await authApi.get<FormFull>(`/election-rounds/${electionRoundId}/forms/${formId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch form');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
  });
};

export function formDetails(electionRoundId: string, formId: string): UseQueryResult<FormFull, Error> {
  return useQuery(formDetailsQueryOptions(electionRoundId, formId));
}

export function useForms(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseQueryResult<PageResponse<FormBase>, Error> {
  return useQuery({
    queryKey: formsKeys.list(electionRoundId, queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };

      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<PageResponse<FormBase>>(`/election-rounds/${electionRoundId}/forms`, {
        params: searchParams
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch forms');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
  });
}
