import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Form, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { InformationCircleIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useNGOMutations } from '../hooks/ngos-quries';
import { newNgoSchema, NGOCreationFormData } from '../models/NGO';

export interface CreateNGODialogProps {
  open: boolean;
  onOpenChange: (open: any) => void;
}

function CreateNGODialog({ open, onOpenChange }: CreateNGODialogProps) {
  const { t } = useTranslation('translation', { keyPrefix: 'observers.addObserver' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { createNgoMutation } = useNGOMutations();

  const form = useForm<NGOCreationFormData>({
    resolver: zodResolver(newNgoSchema),
  });

  function onSubmit(values: NGOCreationFormData) {
    createNgoMutation.mutate({
      values,
      onMutationSuccess: () => {
        form.reset({});
        onOpenChange(false);
        toast({
          title: 'Success',
          description: 'New organization created',
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

              <div className='inline-flex text-slate-700'>
                <div>
                  <InformationCircleIcon width={24} height={24} />
                </div>
                <div className='ml-2 text-sm'>
                  Please add a contact person for this organization. This person will automatically become the
                  organization's first admin.
                </div>
              </div>

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

              <DialogFooter>
                <DialogClose asChild>
                  <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
                    Cancel
                  </Button>
                </DialogClose>
                <Button title={t('addBtnText')} type='submit' className='px-6'>
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
