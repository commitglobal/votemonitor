import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Form, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { toast } from '@/components/ui/use-toast';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { useCreateNgoAdmin } from '../../hooks/ngo-admin-queries';
import { NgoAdminFormData, ngoAdminSchema } from '../../models/NgoAdmin';
import { useCallback, useEffect } from 'react';
import { useBlocker } from '@tanstack/react-router';
import { useConfirm } from '@/components/ui/alert-dialog-provider';

export interface AddNgoAdminDialogProps {
  ngoId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

function AddNgoAdminDialog({ open, onOpenChange, ngoId }: AddNgoAdminDialogProps) {
  const { createNgoAdminMutation } = useCreateNgoAdmin();
  const confirm = useConfirm();

  const form = useForm<NgoAdminFormData>({
    mode: 'all',
    resolver: zodResolver(ngoAdminSchema),
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
          email: '',
          firstName: '',
          lastName: '',
          password: '',
          phoneNumber: '',
        });
      }
      onOpenChange(open);
    },
    [onOpenChange]
  );

  useBlocker({
    shouldBlockFn: async () => {
      if (!form.formState.isDirty) {
        return false;
      }

      return !(await confirm({
        title: `Unsaved Changes Detected`,
        body: 'You have unsaved changes. If you leave this page, your changes will be lost. Are you sure you want to continue?',
        actionButton: 'Leave',
        cancelButton: 'Stay',
      }));
    },
  });

  function onSubmit(values: NgoAdminFormData) {
    createNgoAdminMutation.mutate({
      ngoId,
      values,
      onMutationSuccess: () => {
        form.reset({});
        internalOnOpenChange(false);
        toast({
          title: 'Success',
          description: 'New NGO admin added',
        });
      },
      onMutationError: (error) => {
        error?.errors?.forEach((error) => {
          form.setError(error.name as keyof NgoAdminFormData, { type: 'custom', message: error.reason });
        });

        toast({
          title: 'Error adding NGO admin',
          description: 'Please contact Platform admins',
          variant: 'destructive',
        });
      },
    });
  }

  return (
    <Dialog open={open} onOpenChange={internalOnOpenChange} modal={true}>
      <DialogContent
        className='min-w-[650px] min-h-[350px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
          e.preventDefault();
        }}>
        <DialogTitle className='mb-3.5'>Add NGO admin</DialogTitle>
        <div className='flex flex-col gap-3'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='firstName'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>
                      First name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <Input placeholder='First name' {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='lastName'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>
                      Last name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <Input placeholder='Last name' {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='email'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>
                      Email <span className='text-red-500'>*</span>
                    </FormLabel>
                    <Input placeholder='Email' {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='phoneNumber'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>Phone number</FormLabel>
                    <Input placeholder='Phone number' {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='password'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>
                      Password <span className='text-red-500'>*</span>
                    </FormLabel>
                    <Input placeholder='Password' {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <DialogFooter>
                <DialogClose asChild>
                  <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
                    Cancel
                  </Button>
                </DialogClose>
                <Button title='Add admin' type='submit' className='px-6'>
                  Add admin
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default AddNgoAdminDialog;
