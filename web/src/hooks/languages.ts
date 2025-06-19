import API from '@/services/api';
import { UseQueryResult, useQuery } from '@tanstack/react-query';
import { staticDataKeys } from './query-keys';
import { Language } from '@/common/types';

export const languagesQuery = {
  queryKey: staticDataKeys.languages(),
  queryFn: async () => {
    const response = await API.get<Language[]>('/languages');

    if (response.status !== 200) {
      throw new Error('Failed to fetch languages');
    }

    return response.data;
  },
  refetchOnMount: false,
  staleTime: Number.POSITIVE_INFINITY,
};

export function useLanguages(): UseQueryResult<Language[], Error> {
  return useQuery(languagesQuery);
}
