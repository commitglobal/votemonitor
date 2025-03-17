import { authApi } from '@/common/auth-api';
import { importPollingStationSchema, ReportedError } from '@/common/types';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Form, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { ImportPollingStationRow } from '@/features/polling-stations/PollingStationsImport/PollingStationsImport';
import { pollingStationsKeys } from '@/hooks/polling-stations-levels';
import { sendErrorToSentry } from '@/lib/sentry';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';

export interface CreatePollingStationDialogProps {
  open: boolean;
  onOpenChange: (open: any) => void;
}

function CreatePollingStationDialog({ open, onOpenChange }: CreatePollingStationDialogProps) {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.pollingStations' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const queryClient = useQueryClient();

  const form = useForm<ImportPollingStationRow>({
    mode: 'all',
    resolver: zodResolver(importPollingStationSchema),
  });

  const newPollingStationMutation = useMutation({
    mutationFn: ({ electionRoundId, values }: { electionRoundId: string; values: ImportPollingStationRow }) => {
      console.log(values);
      return authApi.post(`/election-rounds/${electionRoundId}/polling-stations`, values);
    },

    onSuccess: (_, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: t('addPollingStation.onSuccess'),
      });

      queryClient.invalidateQueries({ queryKey: pollingStationsKeys.all(electionRoundId) });

      form.reset({});
      onOpenChange(false);
    },
    onError: (error: ReportedError) => {
      const title = t('addPollingStation.onError');
      sendErrorToSentry({ error, title });
      toast({
        title,
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  function onSubmit(values: ImportPollingStationRow) {
    newPollingStationMutation.mutate({ electionRoundId: currentElectionRoundId, values });
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
        <DialogTitle className='mb-3.5'>{t('addPollingStation.title')}</DialogTitle>
        <div className='flex flex-col gap-3'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='level1'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('headers.level1')}</FormLabel>
                    <Input placeholder={t('headers.level1')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='level2'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('headers.level2')}</FormLabel>
                    <Input placeholder={t('headers.level2')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='level3'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('headers.level3')}</FormLabel>
                    <Input placeholder={t('headers.level3')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='level4'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('headers.level4')}</FormLabel>
                    <Input placeholder={t('headers.level4')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='level5'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('headers.level5')}</FormLabel>
                    <Input placeholder={t('headers.level5')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='number'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('headers.number')}</FormLabel>
                    <Input placeholder={t('headers.number')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='address'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('headers.address')}</FormLabel>
                    <Input placeholder={t('headers.address')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='displayOrder'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('headers.displayOrder')}</FormLabel>
                    <Input placeholder={t('headers.displayOrder')} {...field} {...fieldState} />
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
                <Button title={t('addPollingStation.addBtnText')} type='submit' className='px-6'>
                  {t('addPollingStation.addBtnText')}
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default CreatePollingStationDialog;
