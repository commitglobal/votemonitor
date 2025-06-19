import API from '@/services/api';
import { DataTableParameters, PageResponse } from '@/common/types';
import { buildURLSearchParams } from '@/lib/utils';
import { queryClient } from '@/main';
import { UseQueryResult, queryOptions, useQuery } from '@tanstack/react-query';
import { FormFull, NgoFormBase } from './models';
const STALE_TIME = 1000 * 60 * 5; // five minutes

export const formsKeys = {
  all: (electionRoundId: string) => ['forms', electionRoundId] as const,
  lists: (electionRoundId: string) => [...formsKeys.all(electionRoundId), 'list'] as const,
  list: (electionRoundId: string, params: DataTableParameters) =>
    [...formsKeys.lists(electionRoundId), { ...params }] as const,
  details: (electionRoundId: string) => [...formsKeys.all(electionRoundId), 'detail'] as const,
  detail: (electionRoundId: string, id: string) => [...formsKeys.details(electionRoundId), id] as const,
  baseDetails: (electionRoundId: string, id: string) => [...formsKeys.details(electionRoundId), 'base', id] as const,
};

export const formDetailsQueryOptions = (electionRoundId: string, formId: string) => {
  return queryOptions({
    queryKey: formsKeys.detail(electionRoundId, formId),
    queryFn: async () => {
      const response = await API.get<FormFull>(`/election-rounds/${electionRoundId}/forms/${formId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch form');
      }

      return response.data;
    },
    enabled: !!electionRoundId,
  });
};

export function useForms(
  electionRoundId: string,
  queryParams: DataTableParameters
): UseQueryResult<PageResponse<NgoFormBase>, Error> {
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

      const response = await API.get<PageResponse<NgoFormBase>>(`/election-rounds/${electionRoundId}/forms`, {
        params: searchParams,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch forms');
      }

      response.data.items.forEach((form) => {
        queryClient.setQueryData(formsKeys.baseDetails(electionRoundId, form.id), form);
      });

      return response.data;
    },
    enabled: !!electionRoundId,
    staleTime: STALE_TIME,
  });
}
