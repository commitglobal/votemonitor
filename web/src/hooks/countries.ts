import API from '@/services/api';
import { Country } from '@/common/types';
import { UseQueryResult, useQuery } from '@tanstack/react-query';
import { staticDataKeys } from './query-keys';

export function useCountries(): UseQueryResult<Country[], Error> {
  return useQuery({
    queryKey: staticDataKeys.countries(),
    queryFn: async () => {
      const response = await API.get<Country[]>('/countries');

      if (response.status !== 200) {
        throw new Error('Failed to fetch countries');
      }

      return response.data;
    },
    refetchOnMount: false,
    staleTime: Number.POSITIVE_INFINITY,
  });
}
