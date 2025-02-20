import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Form, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { toast } from '@/components/ui/use-toast';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { useCreateNgo } from '../hooks/ngos-queries';
import { newNgoSchema, NgoCreationFormData } from '../models/NGO';
import { useCallback, useEffect } from 'react';

export interface CreateNGODialogProps {
  open: boolean;
  onOpenChange: (open: any) => void;
}

function CreateNGODialog({ open, onOpenChange }: CreateNGODialogProps) {
  const { createNgoMutation } = useCreateNgo();

  const form = useForm<NgoCreationFormData>({
    resolver: zodResolver(newNgoSchema),
    defaultValues: {
      name: '',
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
          name: '',
        });
      }
      onOpenChange(open);
    },
    [onOpenChange]
  );

  function onSubmit(values: NgoCreationFormData) {
    createNgoMutation.mutate({
      values,
      onMutationSuccess: () => {
        form.reset({});
        internalOnOpenChange(false);
        toast({
          title: 'Success',
          description: 'New organization created',
        });
      },
      onMutationError: (error) => {
        error?.errors?.forEach((error) => {
          form.setError(error.name as keyof NgoCreationFormData, { type: 'custom', message: error.reason });
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
        className='min-w-[650px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
          e.preventDefault();
        }}>
        <DialogTitle className='mb-3.5'>Add organization</DialogTitle>
        <div className='flex flex-col gap-3'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='name'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>Name</FormLabel>
                    <Input placeholder='Name' {...field} {...fieldState} />
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
                <Button title='Add organization' type='submit' className='px-6'>
                  Add organization
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default CreateNGODialog;
