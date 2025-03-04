import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { buttonVariants } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import { buildURLSearchParams, isQueryFiltered } from '@/lib/utils';
import { type UseQueryResult, useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { Observer, ObserverFormData } from '../models/observer';

const STALE_TIME = 1000 * 60 * 5; // five minutes

export const observersKeys = {
  all: () => ['observers'] as const,
  lists: () => [...observersKeys.all(), 'list'] as const,
  list: (params: DataTableParameters) => [...observersKeys.lists(), { ...params }] as const,
  details: () => [...observersKeys.all(), 'detail'] as const,
  detail: (id: string) => [...observersKeys.details(), id] as const,
};

type ObserverResponse = PageResponse<Observer>;

type UseObserversResult = UseQueryResult<ObserverResponse, Error>;

export const useObservers = (queryParams: DataTableParameters): UseObserversResult => {
  return useQuery({
    queryKey: observersKeys.list(queryParams),
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const response = await authApi.get<PageResponse<Observer>>(`/observers`, {
        params: searchParams,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch observers');
      }

      return {
        ...response.data,
        isEmpty: !isQueryFiltered(queryParams.otherParams ?? {}) && response.data.items.length === 0,
      };
    },
    staleTime: STALE_TIME,
  });
};

export const useObserverMutations = () => {
  const router = useRouter();
  const confirm = useConfirm();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const editObserverMutation = useMutation({
    mutationFn: ({ observerId, values }: { observerId: string; values: ObserverFormData }) => {
      return authApi.put(`/observers/${observerId}`, values);
    },

    onSuccess: (_, { observerId }) => {
      queryClient.invalidateQueries({ queryKey: observersKeys.all() });
      router.invalidate();
      navigate({ to: '/observers/$observerId', params: { observerId } });
    },
    onError: () => {
      toast({
        title: 'Error editing NGO admin',
        description: '',
        variant: 'destructive',
      });
    },
  });

  const toggleObserverStatus = useMutation({
    mutationFn: ({ observerId, isObserverActive }: { observerId: string; isObserverActive: boolean }) => {
      const ACTION = isObserverActive ? 'deactivate' : 'activate';

      return authApi.put<any>(`/observers/${observerId}:${ACTION}`, {});
    },

    onSuccess: (_, { isObserverActive }) => {
      queryClient.invalidateQueries({ queryKey: observersKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: `Observer was ${isObserverActive ? 'deactivated' : 'activated'}`,
      });
    },

    onError: (err, { isObserverActive }) => {
      console.error(err);
      toast({
        title: `Error`,
        description: `Error ${isObserverActive ? 'deactivating' : 'activating'} observer`,
        variant: 'destructive',
      });
    },
  });

  const deleteObserverMutation = useMutation({
    mutationFn: ({ observerId }: { observerId: string; onMutationSuccess?: () => void }) => {
      return authApi.delete<any>(`/observers/${observerId}`);
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: observersKeys.all() });
      router.invalidate();

      toast({
        title: 'Success',
        description: 'Observer was deleted successfully',
      });

      if (onMutationSuccess) onMutationSuccess();
    },

    onError: () => {
      toast({
        title: 'Error deleting observer',
        description: '',
        variant: 'destructive',
      });
    },
  });

  // WRAPPED MUTATIONS

  const deleteObserverWithConfirmation = async ({
    observerId,
    name,
    onMutationSuccess,
  }: {
    observerId: string;
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
      deleteObserverMutation.mutate({ observerId, onMutationSuccess });
    }
  };

  return { editObserverMutation, toggleObserverStatus, deleteObserverWithConfirmation };
};
