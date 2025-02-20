import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse, ProblemDetails } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { buttonVariants } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { queryOptions, useMutation, useQuery, UseQueryResult, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { EditNgoFormData, NGO, NgoCreationFormData } from '../models/NGO';
import axios, { AxiosError } from 'axios';

const STALE_TIME = 1000 * 10 * 60; // 10 minutes

export const ngosKeys = {
  all: () => ['ngos'] as const,
  lists: () => [...ngosKeys.all(), 'list'] as const,
  list: (params: DataTableParameters) => [...ngosKeys.lists(), { ...params }] as const,
  details: () => [...ngosKeys.all(), 'detail'] as const,
  detail: (id: string) => [...ngosKeys.details(), id] as const,
  adminDetails: (ngoId: string, ngoAdminId: string) => [...ngosKeys.detail(ngoId), 'admins', ngoAdminId] as const,
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
    staleTime: STALE_TIME,
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
    staleTime: STALE_TIME,
  });

export const useNGODetails = (ngoId: string) => useSuspenseQuery(ngoDetailsOptions(ngoId));

export const useCreateNgo = () => {
  const createNgoMutation = useMutation({
    mutationFn: ({
      values,
    }: {
      values: NgoCreationFormData;
      onMutationSuccess: () => void;
      onMutationError: (error?: ProblemDetails) => void;
    }) => {
      return authApi.post('/ngos', { name: values.name });
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      if (onMutationSuccess) onMutationSuccess();
    },
    onError: (error, { onMutationError }) => {
      if (axios.isAxiosError(error) && error.response) {
        const axiosError = error as AxiosError<ProblemDetails>;

        if (axiosError.response?.status === 400) {
          const problemDetails = axiosError.response.data;
          return onMutationError(problemDetails);
        }
      }

      // Handle non-Axios or unexpected errors
      console.error('Unexpected error:', error);
      onMutationError();
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
      return authApi.put(`/ngos/${ngoId}`, values);
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
      return authApi.post<any>(`/ngos/${ngoId}:deactivate`, {});
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
      return authApi.post<any>(`/ngos/${ngoId}:activate`, {});
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
      return authApi.delete<any>(`/ngos/${ngoId}`);
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
