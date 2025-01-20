import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { queryOptions, useMutation, useQuery, UseQueryResult, useSuspenseQuery } from '@tanstack/react-query';
import { useRouter } from '@tanstack/react-router';
import { NGO } from '../models/NGO';
const ENDPOINT = 'ngos';

export const ngosKeys = {
  all: () => ['ngos'] as const,
  lists: () => [...ngosKeys.all(), 'list'] as const,
  list: (params: DataTableParameters) => [...ngosKeys.lists(), { ...params }] as const,
  details: () => [...ngosKeys.all(), 'detail'] as const,
  detail: (id: string) => [...ngosKeys.details(), id] as const,
};

export function useNGOs(p: DataTableParameters): UseQueryResult<PageResponse<NGO>, Error> {
  return useQuery({
    queryKey: ngosKeys.list(p),
    queryFn: async () => {
      const response = await authApi.get<PageResponse<NGO>>('/ngos', {
        params: {
          ...p.otherParams,
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          SortColumnName: p.sortColumnName,
          SortOrder: p.sortOrder,
        },
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngos');
      }

      return response.data;
    },
  });
}

export const ngoDetailsOptions = (ngoId: string) =>
  queryOptions({
    queryKey: ngosKeys.detail(ngoId),
    queryFn: async () => {
      const response = await authApi.get<NGO>(`/ngos/${ngoId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngo details');
      }

      return response.data;
    },
  });

export const useNGODetails = (ngoId: string) => useSuspenseQuery(ngoDetailsOptions(ngoId));

export const useNGODeactivation = () => {
  const router = useRouter();
  const ngoDeactivationMutation = useMutation({
    mutationFn: (ngoId: string) => {
      return authApi.post<any>(`${ENDPOINT}/${ngoId}:deactivate`, {});
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'NGO was deactivated successfully',
      });
    },

    onError: () => {
      toast({
        title: 'Error deactivating NGO',
        description: '',
        variant: 'destructive',
      });
    },
  });
  return { ngoDeactivationMutation };
};

export const useNGOActivation = () => {
  const router = useRouter();
  const ngoActivationMutation = useMutation({
    mutationFn: (ngoId: string) => {
      return authApi.post<any>(`${ENDPOINT}/${ngoId}:activate`, {});
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'NGO was activated successfully',
      });
    },

    onError: () => {
      toast({
        title: 'Error activating NGO',
        description: '',
        variant: 'destructive',
      });
    },
  });
  return { ngoActivationMutation };
};
