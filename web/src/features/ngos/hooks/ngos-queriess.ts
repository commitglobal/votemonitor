import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { queryOptions, useMutation, useQuery, UseQueryResult, useSuspenseQuery } from '@tanstack/react-query';
import { useRouter } from '@tanstack/react-router';
import { AxiosResponse } from 'axios';
import { NGO, NGOAdminFormData, NGOCreationFormData } from '../models/NGO';
import { NgoAdmin, NgoAdminGetRequestParams } from '../models/NgoAdmin';
const ENDPOINT = 'ngos';

export const ngosKeys = {
  all: () => ['ngos'] as const,
  lists: () => [...ngosKeys.all(), 'list'] as const,
  list: (params: DataTableParameters) => [...ngosKeys.lists(), { ...params }] as const,
  details: () => [...ngosKeys.all(), 'detail'] as const,
  detail: (id: string) => [...ngosKeys.details(), id] as const,
  adminsList: (ngoId: string, params: DataTableParameters) =>
    [...ngosKeys.all(), 'admins', ngoId, { ...params }] as const,
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

export function useNgoAdmins(ngoId: string, p: DataTableParameters): UseQueryResult<PageResponse<NgoAdmin>, Error> {
  return useQuery({
    queryKey: ngosKeys.adminsList(ngoId, p),
    queryFn: async () => {
      const response = await authApi.get<PageResponse<NGO>>(`/ngos/${ngoId}/admins`, {
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

export const useNGOMutations = () => {
  const router = useRouter();
  const createNgoAdminMutation = useMutation({
    mutationFn: ({ ngoId, values }: { ngoId: string; values: NGOAdminFormData; onMutationSuccess: () => void }) => {
      return authApi.post(`${ENDPOINT}/${ngoId}/admins`, values);
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      onMutationSuccess();
    },
    onError: (err) => {
      console.error(err);
    },
  });

  const createNgoMutation = useMutation({
    mutationFn: ({ values }: { values: NGOCreationFormData; onMutationSuccess: () => void }) => {
      return authApi.post(`${ENDPOINT}`, { name: values.name });
    },

    onSuccess: (response: AxiosResponse<NGO>, { values, onMutationSuccess }) => {
      const ngoId = response.data.id;

      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      const { name: _, ...adminValues } = values;

      createNgoAdminMutation.mutate({
        ngoId,
        values: { ...adminValues, password: 'weeetest1234' } as any,
        onMutationSuccess,
      });
    },
    onError: () => {
      toast({
        title: 'Error creating a new NGO',
        description: '',
        variant: 'destructive',
      });
    },
  });

  const deactivateNgoAdminMutation = useMutation({
    mutationFn: ({ ngoId, adminId }: { ngoId: string; adminId: string }) => {
      return authApi.post<any>(`${ENDPOINT}/${ngoId}/admins/${adminId}:deactivate`, {});
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'NGO admin was deactivated successfully',
      });
    },

    onError: () => {
      toast({
        title: 'Error deactivating the NGO admin',
        description: '',
        variant: 'destructive',
      });
    },
  });

  const activateNgoAdminMutation = useMutation({
    mutationFn: ({ ngoId, adminId }: { ngoId: string; adminId: string }) => {
      return authApi.post<any>(`${ENDPOINT}/${ngoId}/admins/${adminId}:activate`, {});
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'NGO admin was activated successfully',
      });
    },

    onError: () => {
      toast({
        title: 'Error activating the NGO admin',
        description: '',
        variant: 'destructive',
      });
    },
  });

  const deleteNgoAdminMutation = useMutation({
    mutationFn: ({ ngoId, adminId }: { ngoId: string; adminId: string }) => {
      return authApi.delete<any>(`${ENDPOINT}/${ngoId}/admins/${adminId}`, {});
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'NGO admin was deleted successfully',
      });
    },

    onError: () => {
      toast({
        title: 'Error deleting NGO admin',
        description: '',
        variant: 'destructive',
      });
    },
  });

  return {
    createNgoMutation,
    createNgoAdminMutation,
    activateNgoAdminMutation,
    deactivateNgoAdminMutation,
    deleteNgoAdminMutation,
  };
};

export const useDeactivateNGO = () => {
  const router = useRouter();
  const deactivateNgoMutation = useMutation({
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
  return { deactivateNgoMutation };
};

export const useActivateNGO = () => {
  const router = useRouter();
  const activateNgoMutation = useMutation({
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
  return { activateNgoMutation };
};

export const useDeteleteNGO = () => {
  const router = useRouter();
  const deleteNgoMutation = useMutation({
    mutationFn: (ngoId: string) => {
      return authApi.delete<any>(`${ENDPOINT}/${ngoId}`);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'NGO was deleted successfully',
      });
    },

    onError: () => {
      toast({
        title: 'Error deleting NGO',
        description: '',
        variant: 'destructive',
      });
    },
  });
  return { deleteNgoMutation };
};

export const ngoAdminDetailsOptions = ({ ngoId, adminId }: NgoAdminGetRequestParams) =>
  queryOptions({
    queryKey: ngosKeys.detail(ngoId),
    queryFn: async () => {
      const response = await authApi.get<NgoAdmin>(`/ngos/${ngoId}/admins/${adminId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngo details');
      }

      return response.data;
    },
  });

export const useNgoAdminDetails = ({ ngoId, adminId }: NgoAdminGetRequestParams) =>
  useSuspenseQuery(ngoAdminDetailsOptions({ ngoId, adminId }));
