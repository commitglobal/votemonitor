import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Form, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { toast } from '@/components/ui/use-toast';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useNGOMutations } from '../../hooks/ngos-queriess';
import { NgoAdminFormData, ngoAdminSchema } from '../../models/NgoAdmin';

export interface AddNgoAdminDialogProps {
  ngoId: string;
  open: boolean;
  onOpenChange: (open: any) => void;
}

function AddNgoAdminDialog({ open, onOpenChange, ngoId }: AddNgoAdminDialogProps) {
  const { t } = useTranslation('translation', { keyPrefix: 'observers.addObserver' });
  const { createNgoAdminMutation } = useNGOMutations();

  const form = useForm<NgoAdminFormData>({
    resolver: zodResolver(ngoAdminSchema),
  });

  function onSubmit(values: NgoAdminFormData) {
    createNgoAdminMutation.mutate({
      ngoId,
      values,
      onMutationSuccess: () => {
        form.reset({});
        onOpenChange(false);
        toast({
          title: 'Success',
          description: 'New NGO admin added',
        });
      },
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
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
                    <FormLabel>{t('firstName')}</FormLabel>
                    <Input placeholder={t('firstName')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='lastName'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('lastName')}</FormLabel>
                    <Input placeholder={t('lastName')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='email'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('email')}</FormLabel>
                    <Input placeholder={t('email')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='phoneNumber'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('phone')}</FormLabel>
                    <Input placeholder={t('phone')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='password'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>Password</FormLabel>
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
