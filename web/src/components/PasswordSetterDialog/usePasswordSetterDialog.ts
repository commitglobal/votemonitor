import { authApi } from '@/common/auth-api';
import { addFormValidationErrorsFromBackend } from '@/common/form-backend-validation';
import { ProblemDetails } from '@/common/types';
import { sendErrorToSentry } from '@/lib/sentry';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { useCallback, useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useDialog } from '../ui/use-dialog';
import { toast } from '../ui/use-toast';

const passwordSetterSchema = z
  .object({
    newPassword: z.string().min(8, 'Password must be at least 8 characters long'),
    confirmNewPassword: z.string(),
  })
  .refine((data) => data.newPassword === data.confirmNewPassword, {
    message: 'Passwords do not match',
    path: ['confirmNewPassword'],
  });

type PasswordSetterUserInfoParams = {
  userId: string;
  displayName: string;
};

export type PasswordSetterFormData = z.infer<typeof passwordSetterSchema>;

export const usePasswordSetterDialog = () => {
  const [userId, setUserId] = useState<string>('');
  const [displayName, setDisplayName] = useState<string>('');
  const passwordSetterDialog = useDialog();
  const { open, onOpenChange } = passwordSetterDialog.dialogProps;

  const form = useForm<PasswordSetterFormData>({
    resolver: zodResolver(passwordSetterSchema),
    defaultValues: {
      newPassword: '',
      confirmNewPassword: '',
    },
  });

  useEffect(() => {
    if (form.formState.isSubmitSuccessful) {
      form.reset(undefined, { keepValues: true });
    }
  }, [form.formState.isSubmitSuccessful]);

  const internalOnOpenChange = useCallback(
    (open: boolean) => {
      if (!open) {
        form.reset({
          newPassword: '',
          confirmNewPassword: '',
        });
      }
      onOpenChange(open);
    },
    [onOpenChange]
  );

  const setPasswordMutation = useMutation({
    mutationFn: (data: PasswordSetterFormData) => {
      return authApi.post<PasswordSetterFormData>(`auth/set-password`, {
        aspNetUserId: userId,
        newPassword: data.newPassword,
      });
    },
    onSuccess: () => {
      form.reset({});
      internalOnOpenChange(false);
      toast({
        title: 'Success',
        description: 'Password set',
      });
    },

    onError: (error: AxiosError<ProblemDetails>) => {
      const title = 'Error setting password';
      addFormValidationErrorsFromBackend<PasswordSetterFormData>(form, error);
      sendErrorToSentry({ error, title });
      toast({
        title,
        description: 'Please contact Platform admins',
        variant: 'destructive',
      });
    },
  });

  const handlePasswordSet = (params: PasswordSetterUserInfoParams) => {
    setUserId(params.userId);
    setDisplayName(params.displayName);
    passwordSetterDialog.trigger();
  };

  const onSubmit = (values: PasswordSetterFormData) => {
    setPasswordMutation.mutate(values);
  };

  const passwordSetterDialogProps = { open, form, userId, displayName, onOpenChange, internalOnOpenChange, onSubmit };

  return {
    passwordSetterDialogProps,
    handlePasswordSet,
  };
};
