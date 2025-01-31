import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { buttonVariants } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { queryOptions, useMutation, useQuery, UseQueryResult, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { AxiosResponse } from 'axios';
import { EditNgoFormData, NGO, NgoCreationFormData } from '../models/NGO';
import { useCreateNgoAdmin } from './ngo-admin-queries';
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
        params: { ...p.otherParams },
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

export const useCreateNgo = () => {
  const { createNgoAdminMutation } = useCreateNgoAdmin();

  const createNgoMutation = useMutation({
    mutationFn: ({ values }: { values: NgoCreationFormData; onMutationSuccess: () => void }) => {
      return authApi.post(`${ENDPOINT}`, { name: values.name });
    },

    onSuccess: (response: AxiosResponse<NGO>, { values, onMutationSuccess }) => {
      const ngoId = response.data.id;

      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      const { name: _, ...adminValues } = values;

      createNgoAdminMutation.mutate({
        ngoId,
        values: adminValues,
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

  return {
    createNgoMutation,
  };
};

export const useNgoMutations = () => {
  const router = useRouter();
  const confirm = useConfirm();
  const navigate = useNavigate();

  const editNgoMutation = useMutation({
    mutationFn: ({ ngoId, values }: { ngoId: string; values: EditNgoFormData }) => {
      return authApi.put(`${ENDPOINT}/${ngoId}`, values);
    },

    onSuccess: (_, { ngoId }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();
      navigate({ to: '/ngos/view/$ngoId/$tab', params: { ngoId: ngoId!, tab: 'details' } });
    },
    onError: () => {
      toast({
        title: 'Error editing NGO',
        description: '',
        variant: 'destructive',
      });
    },
  });

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

  const deleteNgoMutation = useMutation({
    mutationFn: ({ ngoId }: { ngoId: string; onMutationSuccess?: () => void }) => {
      return authApi.delete<any>(`${ENDPOINT}/${ngoId}`);
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'NGO was deleted successfully',
      });

      if (onMutationSuccess) onMutationSuccess();
    },

    onError: () => {
      toast({
        title: 'Error deleting NGO',
        description: '',
        variant: 'destructive',
      });
    },
  });

  // WRAPPED MUTATIONS

  const deleteNgoWithConfirmation = async ({
    ngoId,
    name,
    onMutationSuccess,
  }: {
    ngoId: string;
    name: string;
    onMutationSuccess?: () => void;
  }) => {
    if (
      await confirm({
        title: `Delete ${name}?`,
        body: 'This action is permanent and cannot be undone. Once deleted, this organization cannot be retrieved.',
        actionButton: 'Delete',
        actionButtonClass: buttonVariants({ variant: 'destructive' }),
        cancelButton: 'Cancel',
      })
    ) {
      deleteNgoMutation.mutate({ ngoId, onMutationSuccess });
    }
  };

  return { editNgoMutation, deactivateNgoMutation, activateNgoMutation, deleteNgoWithConfirmation };
};
