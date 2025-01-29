import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { buttonVariants } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import {
  queryOptions,
  useMutation,
  useQuery,
  useQueryClient,
  UseQueryResult,
  useSuspenseQuery,
} from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { EditNgoAdminFormData, NgoAdmin, NgoAdminFormData, NgoAdminGetRequestParams } from '../models/NgoAdmin';
import { ngosKeys } from './ngos-queriess';

const getEndpointWithNgoId = (ngoId: string): string => {
  if (!ngoId) throw new Error('No NGO ID provided');

  return `ngos/${ngoId}/admins`;
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

export function useNgoAdmins(ngoId: string, p: DataTableParameters): UseQueryResult<PageResponse<NgoAdmin>, Error> {
  return useQuery({
    queryKey: ngosKeys.adminsList(ngoId, p),
    queryFn: async () => {
      const response = await authApi.get<PageResponse<NgoAdmin>>(`/ngos/${ngoId}/admins`, {
        params: {
          ...p.otherParams,
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          SortColumnName: p.sortColumnName,
          SortOrder: p.sortOrder,
        },
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngo admins');
      }

      return response.data;
    },
  });
}

// NGO ADMIN CREATION MUTATION

export const useCreateNgoAdmin = () => {
  const queryClient = useQueryClient();
  const createNgoAdminMutation = useMutation({
    mutationFn: ({ ngoId, values }: { ngoId: string; values: NgoAdminFormData; onMutationSuccess: () => void }) => {
      return authApi.post(getEndpointWithNgoId(ngoId), values);
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      onMutationSuccess();
    },
    onError: (err) => {
      console.error(err);
    },
  });

  return { createNgoAdminMutation };
};

export const useNgoAdminMutations = (ngoId: string) => {
  const DEFAULT_ENDPOINT = getEndpointWithNgoId(ngoId);
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const router = useRouter();
  const confirm = useConfirm();

  // MUTATIONS

  const editNgoAdminMutation = useMutation({
    mutationFn: ({ adminId, values }: { adminId: string; values: EditNgoAdminFormData }) => {
      return authApi.put(`${DEFAULT_ENDPOINT}/${adminId}`, values);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();
      navigate({ to: '/ngos/view/$ngoId/$tab', params: { ngoId: ngoId!, tab: 'admins' } });
    },
    onError: () => {
      toast({
        title: 'Error editing NGO admin',
        description: '',
        variant: 'destructive',
      });
    },
  });

  const deleteNgoAdminMutation = useMutation({
    mutationFn: ({ adminId }: { adminId: string; onMutationSuccess?: () => void }) => {
      return authApi.delete<any>(`${DEFAULT_ENDPOINT}/${adminId}`, {});
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'NGO admin was deleted successfully',
      });

      if (onMutationSuccess) onMutationSuccess();
    },

    onError: () => {
      toast({
        title: 'Error deleting NGO admin',
        description: '',
        variant: 'destructive',
      });
    },
  });

  const deactivateNgoAdminMutation = useMutation({
    mutationFn: (adminId: string) => {
      return authApi.post<any>(`${DEFAULT_ENDPOINT}/${adminId}:deactivate`, {});
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
    mutationFn: (adminId: string) => {
      return authApi.post<any>(`${DEFAULT_ENDPOINT}/${adminId}:activate`, {});
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

  // WRAPPED MUTATIONS

  const deleteNgoAdminWithConfirmation = async ({
    adminId,
    name,
    onMutationSuccess,
  }: {
    adminId: string;
    name: string;
    onMutationSuccess?: () => void;
  }) => {
    if (
      await confirm({
        title: `Delete ${name}?`,
        body: 'This action is permanent and cannot be undone. Once deleted, this NGO admin cannot be retrieved.',
        actionButton: 'Delete',
        actionButtonClass: buttonVariants({ variant: 'destructive' }),
        cancelButton: 'Cancel',
      })
    ) {
      deleteNgoAdminMutation.mutate({ adminId, onMutationSuccess });
    }
  };

  return {
    editNgoAdminMutation,
    deactivateNgoAdminMutation,
    activateNgoAdminMutation,
    deleteNgoAdminWithConfirmation,
  };
};
