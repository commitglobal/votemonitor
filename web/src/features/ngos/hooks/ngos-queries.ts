import { DataTableParameters, PageResponse, ProblemDetails } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { buttonVariants } from '@/components/ui/button';
import { queryClient } from '@/main';
import API from '@/services/api';
import { queryOptions, useMutation, useQuery, UseQueryResult, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import axios, { AxiosError } from 'axios';
import { toast } from 'sonner';
import { EditNgoFormData, NGO, NgoCreationFormData } from '../models/NGO';

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
      const response = await API.get<PageResponse<NGO>>('/ngos', {
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
      const response = await API.get<NGO>(`/ngos/${ngoId}`);

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
      return API.post('/ngos', { name: values.name });
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
      toast.error('Error creating a new NGO');
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
      return API.put(`/ngos/${ngoId}`, values);
    },

    onSuccess: (_, { ngoId }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();
      navigate({ to: '/ngos/view/$ngoId/$tab', params: { ngoId: ngoId!, tab: 'details' } });
    },
    onError: () => {
      toast.error('Error editing NGO');
    },
  });

  const deactivateNgoMutation = useMutation({
    mutationFn: (ngoId: string) => {
      return API.post<any>(`/ngos/${ngoId}:deactivate`, {});
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast.success('NGO was deactivated successfully');
    },

    onError: () => {
      toast.error('Error deactivating NGO');
    },
  });

  const activateNgoMutation = useMutation({
    mutationFn: (ngoId: string) => {
      return API.post<any>(`/ngos/${ngoId}:activate`, {});
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast.success('NGO was activated successfully');
    },

    onError: () => {
      toast.error('Error activating NGO');
    },
  });

  const deleteNgoMutation = useMutation({
    mutationFn: ({ ngoId }: { ngoId: string; onMutationSuccess?: () => void }) => {
      return API.delete<any>(`/ngos/${ngoId}`);
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast.success('NGO was deleted successfully');

      if (onMutationSuccess) onMutationSuccess();
    },

    onError: () => {
      toast.error('Error deleting NGO');
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
