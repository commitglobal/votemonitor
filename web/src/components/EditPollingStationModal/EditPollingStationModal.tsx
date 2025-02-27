import { importPollingStationSchema, PollingStation } from '@/common/types';
import { Button } from '@/components/ui/button';
import { DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect } from 'react';
import { useForm } from 'react-hook-form';

type EditPollingStationModalProps = {
  pollingStation: PollingStation;
  updatePollingStation: (pollingStation: PollingStation) => void;
};

export default function EditPollingStationModal({
  pollingStation,
  updatePollingStation,
}: EditPollingStationModalProps) {
  const form = useForm<PollingStation>({
    resolver: zodResolver(importPollingStationSchema),
    mode: 'onChange',
    reValidateMode: 'onChange',
    defaultValues: {
      id: pollingStation.id,
      level1: pollingStation.level1,
      level2: pollingStation.level2,
      level3: pollingStation.level3,
      level4: pollingStation.level4,
      level5: pollingStation.level5,
      number: pollingStation.number,
      address: pollingStation.address,
      displayOrder: pollingStation.displayOrder,
      tags: pollingStation.tags,
    },
  });

  useEffect(() => {
    form.trigger();
  }, [form.trigger]);

  function onSubmit(updatedPollingStation: PollingStation) {
    updatePollingStation(updatedPollingStation);
  }

  return (
    <>
      <DialogHeader>
        <DialogTitle>Edit polling station</DialogTitle>
      </DialogHeader>
      <div className='py-2'>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className='grid gap-2'>
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

            <FormField
              control={form.control}
              name='address'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>Address</FormLabel>
                  <FormControl>
                    <Input type='text' {...field} {...fieldState} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name='displayOrder'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>Display order</FormLabel>
                  <FormControl>
                    <Input type='number' {...field} {...fieldState} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <Button type='submit' className='mt-2 w-full'>
              Update Details
            </Button>
          </form>
        </Form>
      </div>
    </>
  );
}
