import { authApi } from '@/common/auth-api';
import { importPollingStationSchema } from '@/common/types';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { ImportPollingStationRow } from '@/features/polling-stations/PollingStationsImport/PollingStationsImport';
import { pollingStationsKeys } from '@/hooks/polling-stations-levels';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { toast } from 'sonner';

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
      return authApi.post(`/election-rounds/${electionRoundId}/polling-stations`, values);
    },

    onSuccess: (_, { electionRoundId }) => {
      toast(t('addPollingStation.onSuccess'));

      queryClient.invalidateQueries({ queryKey: pollingStationsKeys.all(electionRoundId) });

      form.reset({});
      onOpenChange(false);
    },
    onError: (err) => {
      toast.error(t('addPollingStation.onError'),{
        description: 'Please contact tech support',
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
              <div className='grid grid-cols-2 gap-4'>
                <FormField
                  control={form.control}
                  name='level1'
                  render={({ field, fieldState }) => (
                    <FormItem>
                      <FormLabel>Level 1</FormLabel>
                      <FormControl>
                        <Input type='text' {...field} {...fieldState} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name='level2'
                  render={({ field, fieldState }) => (
                    <FormItem>
                      <FormLabel>Level 2</FormLabel>
                      <FormControl>
                        <Input type='text' {...field} {...fieldState} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name='level3'
                  render={({ field, fieldState }) => (
                    <FormItem>
                      <FormLabel>Level 3</FormLabel>
                      <FormControl>
                        <Input type='text' {...field} {...fieldState} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name='level4'
                  render={({ field, fieldState }) => (
                    <FormItem>
                      <FormLabel>Level 4</FormLabel>
                      <FormControl>
                        <Input type='text' {...field} {...fieldState} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name='level5'
                  render={({ field, fieldState }) => (
                    <FormItem>
                      <FormLabel>Level 5</FormLabel>
                      <FormControl>
                        <Input type='text' {...field} {...fieldState} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name='number'
                  render={({ field, fieldState }) => (
                    <FormItem>
                      <FormLabel>Number</FormLabel>
                      <FormControl>
                        <Input type='text' {...field} {...fieldState} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
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

              <div className='grid grid-cols-2 gap-4'>
                <FormField
                  control={form.control}
                  name='latitude'
                  render={({ field, fieldState }) => (
                    <FormItem>
                      <FormLabel>Latitude</FormLabel>
                      <Input
                        type='number'
                        step='any'
                        placeholder='-90 to 90'
                        {...field}
                        {...fieldState}
                        value={field.value ?? ''}
                        onChange={(e) => field.onChange(e.target.value ? parseFloat(e.target.value) : undefined)}
                      />
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name='longitude'
                  render={({ field, fieldState }) => (
                    <FormItem>
                      <FormLabel>Longitude</FormLabel>
                      <Input
                        type='number'
                        step='any'
                        placeholder='-180 to 180'
                        {...field}
                        {...fieldState}
                        value={field.value ?? ''}
                        onChange={(e) => field.onChange(e.target.value ? parseFloat(e.target.value) : undefined)}
                      />
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

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
