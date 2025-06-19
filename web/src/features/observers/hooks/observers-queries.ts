import { addFormValidationErrorsFromBackend } from '@/common/form-backend-validation';
import type { DataTableParameters, PageResponse, ProblemDetails } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { buttonVariants } from '@/components/ui/button';
import API from '@/services/api';
import { type UseQueryResult, useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { AxiosError } from 'axios';
import { UseFormReturn } from 'react-hook-form';
import { toast } from 'sonner';
import { AddObserverFormData, EditObserverFormData, Observer } from '../models/observer';

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
      const response = await API.get<PageResponse<Observer>>('/observers', {
        params: {
          ...queryParams.otherParams,
          status: (queryParams.otherParams as any)?.observerStatus,
        },
      });
      if (response.status !== 200) {
        throw new Error('Failed to fetch observers');
      }

      return response.data;
    },
    staleTime: STALE_TIME,
  });
};

export const useObserverMutations = () => {
  const router = useRouter();
  const confirm = useConfirm();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const addObserverMutation = useMutation({
    mutationFn: ({
      values,
    }: {
      values: AddObserverFormData;
      form: UseFormReturn<AddObserverFormData>;
      onOpenChange: (isOpen: boolean) => void;
    }) => {
      return API.post(`/observers`, values);
    },

    onSuccess: (_, { form, onOpenChange }) => {
      toast.success('Observer added');
      queryClient.invalidateQueries({ queryKey: observersKeys.all() });
      router.invalidate();
      form.reset({});
      onOpenChange(false);
    },
    onError: (error: AxiosError<ProblemDetails>, { form }) => {
      console.error(error);
      addFormValidationErrorsFromBackend(form, error);
      toast.error('Error adding observer');
    },
  });

  const editObserverMutation = useMutation({
    mutationFn: ({
      observerId,
      values,
    }: {
      observerId: string;
      values: EditObserverFormData;
      form: UseFormReturn<EditObserverFormData>;
    }) => {
      return API.put(`/observers/${observerId}`, values);
    },

    onSuccess: (_, { observerId }) => {
      queryClient.invalidateQueries({ queryKey: observersKeys.all() });
      router.invalidate();
      navigate({ to: '/observers/$observerId', params: { observerId } });
    },
    onError: (error: AxiosError<ProblemDetails>, { form }) => {
      console.error(error);
      addFormValidationErrorsFromBackend(form, error);

      toast.error('Error editing observer');
    },
  });

  const toggleObserverStatus = useMutation({
    mutationFn: ({ observerId, isObserverActive }: { observerId: string; isObserverActive: boolean }) => {
      const ACTION = isObserverActive ? 'deactivate' : 'activate';

      return API.put<any>(`/observers/${observerId}:${ACTION}`, {});
    },

    onSuccess: (_, { isObserverActive }) => {
      queryClient.invalidateQueries({ queryKey: observersKeys.all() });
      router.invalidate();

      toast.success(`Observer was ${isObserverActive ? 'deactivated' : 'activated'}`);
    },

    onError: (err, { isObserverActive }) => {
      console.error(err);
      toast.error(`Error ${isObserverActive ? 'deactivating' : 'activating'} observer`);
    },
  });

  const deleteObserverMutation = useMutation({
    mutationFn: ({ observerId }: { observerId: string; onMutationSuccess?: () => void }) => {
      return API.delete<any>(`/observers/${observerId}`);
    },

    onSuccess: (_, { onMutationSuccess }) => {
      queryClient.invalidateQueries({ queryKey: observersKeys.all() });
      router.invalidate();

      toast.success('Observer was deleted successfully');

      if (onMutationSuccess) onMutationSuccess();
    },

    onError: () => {
      toast.error('Error deleting observer');
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

  return { addObserverMutation, editObserverMutation, toggleObserverStatus, deleteObserverWithConfirmation };
};
