import { authApi } from '@/common/auth-api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import TagsSelectFormField from '@/components/ui/tag-selector';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';
import { monitoringObserversKeys } from '../../hooks/monitoring-observers-queries';

export interface CreateMonitoringObserverDialogProps {
  open: boolean;
  onOpenChange: (open: any) => void;
}

function CreateMonitoringObserverDialog({ open, onOpenChange }: CreateMonitoringObserverDialogProps) {
  const { t } = useTranslation('translation', { keyPrefix: 'observers.addObserver' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: availableTags } = useMonitoringObserversTags(currentElectionRoundId);
  const queryClient = useQueryClient();
  const newObserverSchema = z.object({
    firstName: z.string(),
    lastName: z.string(),
    email: z.string().email(),
    phoneNumber: z.string(),
    tags: z.any(),
  });

  type ObserverFormData = z.infer<typeof newObserverSchema>;

  const form = useForm<ObserverFormData>({
    resolver: zodResolver(newObserverSchema),
  });

  function onSubmit(values: ObserverFormData) {
    newObserverMutation.mutate({ electionRoundId: currentElectionRoundId, values });
  }

  const newObserverMutation = useMutation({
    mutationFn: ({ electionRoundId, values }: { electionRoundId: string; values: ObserverFormData }) => {
      return authApi.post(`/election-rounds/${electionRoundId}/monitoring-observers`, { observers: [values] });
    },

    onSuccess: (_, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: t('onSuccess'),
      });

      queryClient.invalidateQueries({ queryKey: monitoringObserversKeys.all(electionRoundId) });
      form.reset({});
      onOpenChange(false);
    },
    onError: () => {
      toast({
        title: t('onError'),
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

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
        <DialogTitle className='mb-3.5'>{t('title')}</DialogTitle>
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
                name='tags'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className='text-left'>Tags</FormLabel>
                    <FormControl>
                      <TagsSelectFormField
                        options={availableTags?.filter((tag) => !field.value?.includes(tag)) ?? []}
                        defaultValue={field.value}
                        onValueChange={field.onChange}
                        placeholder='Observer tags'
                      />
                    </FormControl>
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
                  {t('addBtnText')}
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default CreateMonitoringObserverDialog;
