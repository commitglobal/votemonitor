import { importLocationSchema, Location } from '@/common/types';
import { Button } from '@/components/ui/button';
import { DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect } from 'react';
import { useForm } from 'react-hook-form';

type EditLocationModalProps = {
  location: Location;
  updateLocation: (location: Location) => void;
};

export default function EditLocationModal({ location, updateLocation }: EditLocationModalProps) {
  const form = useForm<Location>({
    resolver: zodResolver(importLocationSchema),
    mode: 'onChange',
    reValidateMode: 'onChange',
    defaultValues: {
      id: location.id,
      level1: location.level1,
      level2: location.level2,
      level3: location.level3,
      level4: location.level4,
      level5: location.level5,
      displayOrder: location.displayOrder,
      tags: location.tags,
    },
  });

  useEffect(() => {
    form.trigger();
  }, [form.trigger]);

  function onSubmit(updatedLocation: Location) {
    updateLocation(updatedLocation);
  }

  return (
    <>
      <DialogHeader>
        <DialogTitle>Edit location</DialogTitle>
      </DialogHeader>
      <div className='py-4'>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className='grid gap-4'>
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
