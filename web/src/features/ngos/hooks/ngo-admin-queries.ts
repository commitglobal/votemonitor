import { authApi } from '@/common/auth-api';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { buttonVariants } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import { queryOptions, useMutation, useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { EditNgoAdminFormData, NgoAdmin, NgoAdminFormData, NgoAdminGetRequestParams } from '../models/NgoAdmin';
import { ngosKeys } from './ngos-queriess';

// Returns endpoint with custom id, useful for creating NGO Admins along with NGOs

const getEndpointWithNgoId = (ngoId: string) => {
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

export const useNgoAdminMutations = (ngoId?: string) => {
  const DEFAULT_ENDPOINT = `ngos/${ngoId}/admins`;
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

  const createNgoAdminMutation = useMutation({
    mutationFn: ({ ngoId, values }: { ngoId: string; values: NgoAdminFormData; onMutationSuccess: () => void }) => {
      return authApi.post(ngoId ? getEndpointWithNgoId(ngoId) : `${DEFAULT_ENDPOINT}`, values);
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: ngosKeys.all() });
      onMutationSuccess();
    },
    onError: (err) => {
      console.error(err);
    },
  });

  // WRAPPED MUTATIONS

  const deleteNgoAdminWithConfirmation = async ({
    ngoId,
    adminId,
    name,
    onMutationSuccess,
  }: {
    ngoId: string;
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

  return { createNgoAdminMutation, editNgoAdminMutation, deleteNgoAdminWithConfirmation };
};
