import { zodResolver } from '@hookform/resolvers/zod';
import { useQueryClient } from '@tanstack/react-query';
import { Plus, Trash2 } from 'lucide-react';
import { useCallback, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';

import { importPollingStationSchema, PollingStation } from '@/common/types';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { ImportPollingStationRow } from '@/features/polling-stations/PollingStationsImport/PollingStationsImport';
import { pollingStationsKeys } from '@/hooks/polling-stations-levels';
import { toast } from 'sonner';
import { useUpsertPollingStation } from '../PollingStationsDashboard/hooks';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../ui/tabs';

interface UpsertPollingStationDialogProps {
  pollingStation?: PollingStation;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const DEFAULT_FORM_VALUES: Partial<PollingStation> = {
  level1: '',
  level2: '',
  level3: '',
  level4: '',
  level5: '',
  address: '',
  number: '',
  displayOrder: undefined,
  latitude: undefined,
  longitude: undefined,
  tags: {},
};

export default function UpsertPollingStationDialog({
  pollingStation,
  open,
  onOpenChange,
}: UpsertPollingStationDialogProps) {
  const { t } = useTranslation('translation', { keyPrefix: 'electionEvent.pollingStations' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const queryClient = useQueryClient();
  const [tagKey, setTagKey] = useState('');
  const [tagValue, setTagValue] = useState('');

  const form = useForm<ImportPollingStationRow>({
    defaultValues: {
      id: pollingStation?.id,
      level1: pollingStation?.level1 ?? '',
      level2: pollingStation?.level2 ?? '',
      level3: pollingStation?.level3 ?? '',
      level4: pollingStation?.level4 ?? '',
      level5: pollingStation?.level5 ?? '',
      number: pollingStation?.number ?? '',
      address: pollingStation?.address ?? '',
      displayOrder: pollingStation?.displayOrder ?? undefined,
      latitude: pollingStation?.latitude ?? undefined,
      longitude: pollingStation?.longitude ?? undefined,
      tags: pollingStation?.tags ?? {},
    },
    mode: 'onChange',
    resolver: zodResolver(importPollingStationSchema),
  });
  const onSuccess = useCallback(() => {
    toast.success(t('addPollingStation.onSuccess'));

    queryClient.invalidateQueries({ queryKey: pollingStationsKeys.all(currentElectionRoundId) });
    form.reset(DEFAULT_FORM_VALUES);
    onOpenChange(false);
  }, [queryClient, form, currentElectionRoundId]);

  const onError = useCallback(() => {
    toast.error(t('addPollingStation.onError'), {
      description: 'Please contact tech support',
    });
  }, [queryClient, form, currentElectionRoundId]);

  const { mutate: upsertPollingStation, isPending } = useUpsertPollingStation(onSuccess, onError);

  const handleSubmit = (values: ImportPollingStationRow) => {
    upsertPollingStation({ electionRoundId: currentElectionRoundId, values });
  };

  function addTag() {
    if (tagKey.trim() !== '') {
      const updatedTags = {
        ...form.getValues('tags'),
        [tagKey]: tagValue,
      };
      form.setValue('tags', updatedTags);
      setTagKey('');
      setTagValue('');
    }
  }

  function removeTag(key: string) {
    const updatedTags = { ...form.getValues('tags') };
    delete updatedTags[key];
    form.setValue('tags', updatedTags);
  }

  function handleOnOpenChange(open: boolean) {
    form.reset(DEFAULT_FORM_VALUES);
    onOpenChange(open);
  }

  return (
    <Dialog open={open} onOpenChange={handleOnOpenChange} modal>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t('addPollingStation.title')}</DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className='space-y-4'>
            <Tabs defaultValue='basic' className='w-full'>
              <TabsList className='grid w-full grid-cols-3'>
                <TabsTrigger value='basic'>Basic Info</TabsTrigger>
                <TabsTrigger value='location'>Location</TabsTrigger>
                <TabsTrigger value='advanced'>Advanced</TabsTrigger>
              </TabsList>

              {/* Basic Info Tab */}
              <TabsContent value='basic' className='space-y-4 pt-4'>
                <div className='space-y-4'>
                  {/* Level fields - 2 rows, 3 columns */}
                  <div className='grid grid-cols-1 sm:grid-cols-3 gap-4'>
                    <FormField
                      control={form.control}
                      name='level1'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Level 1 *</FormLabel>
                          <FormControl>
                            <Input {...field} placeholder='Level1' />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name='level2'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Level 2</FormLabel>
                          <FormControl>
                            <Input {...field} placeholder='Level2' />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name='level3'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Level 3</FormLabel>
                          <FormControl>
                            <Input {...field} placeholder='Level3' />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>

                  <div className='grid grid-cols-1 sm:grid-cols-3 gap-4'>
                    <FormField
                      control={form.control}
                      name='level4'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Level 4</FormLabel>
                          <FormControl>
                            <Input {...field} placeholder='Level4' />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name='level5'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Level 5</FormLabel>
                          <FormControl>
                            <Input {...field} placeholder='Level5' />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={form.control}
                      name='number'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Number *</FormLabel>
                          <FormControl>
                            <Input {...field} placeholder='Polling station number' />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>

                  {/* Address field - full width */}
                  <FormField
                    control={form.control}
                    name='address'
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Address *</FormLabel>
                        <FormControl>
                          <Input {...field} placeholder='Full address' />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </TabsContent>

              {/* Location Tab */}
              <TabsContent value='location' className='space-y-4 pt-4'>
                <div className='space-y-4'>
                  <div className='grid grid-cols-1 sm:grid-cols-2 gap-4'>
                    <FormField
                      control={form.control}
                      name='latitude'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Latitude</FormLabel>
                          <FormControl>
                            <Input
                              type='number'
                              step='0.000001'
                              {...field}
                              value={field.value || ''}
                              onChange={(e) => field.onChange(e.target.value ? Number(e.target.value) : undefined)}
                              placeholder='e.g. 40.7128'
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name='longitude'
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Longitude</FormLabel>
                          <FormControl>
                            <Input
                              type='number'
                              step='0.000001'
                              {...field}
                              value={field.value || ''}
                              onChange={(e) => field.onChange(e.target.value ? Number(e.target.value) : undefined)}
                              placeholder='e.g. -74.0060'
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </div>
                <FormField
                  control={form.control}
                  name='displayOrder'
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Display Order</FormLabel>
                      <FormControl>
                        <Input
                          type='number'
                          {...field}
                          onChange={(e) => field.onChange(Number(e.target.value))}
                          placeholder='e.g. 1'
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </TabsContent>

              {/* Advanced Tab */}
              <TabsContent value='advanced' className='space-y-4 pt-4'>
                <FormField
                  control={form.control}
                  name='tags'
                  render={({ field }) => {
                    return (
                      <FormItem className='space-y-4'>
                        <FormLabel>Tags</FormLabel>
                        <FormDescription>Add key-value pairs as tags for this polling station</FormDescription>
                        <FormControl>
                          <div className='space-y-4'>
                            <div className='flex gap-2'>
                              <Input
                                placeholder='Key'
                                value={tagKey}
                                onChange={(e) => setTagKey(e.target.value)}
                                className='flex-1'
                              />
                              <Input
                                placeholder='Value'
                                value={tagValue}
                                onChange={(e) => setTagValue(e.target.value)}
                                className='flex-1'
                              />
                              <Button type='button' onClick={addTag} size='icon'>
                                <Plus className='h-4 w-4' />
                              </Button>
                            </div>

                            {Object.keys(field.value || {}).length > 0 && (
                              <div className='border rounded-md p-3'>
                                <div className='text-sm font-medium mb-2'>Current Tags</div>
                                <div className='space-y-2'>
                                  {Object.entries(field.value || {}).map(([key, value]) => (
                                    <div
                                      key={key}
                                      className='flex items-center justify-between bg-muted p-2 rounded-md'>
                                      <div>
                                        <span className='font-medium'>{key}:</span> {value}
                                      </div>
                                      <Button type='button' variant='ghost' size='icon' onClick={() => removeTag(key)}>
                                        <Trash2 className='h-4 w-4' />
                                      </Button>
                                    </div>
                                  ))}
                                </div>
                              </div>
                            )}
                          </div>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    );
                  }}
                />
              </TabsContent>
            </Tabs>

            <DialogFooter className='pt-2'>
              <Button type='submit' disabled={isPending}>
                Save Polling Station
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
