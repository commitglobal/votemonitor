
import { Button } from '@/components/ui/button';
import { DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { ImportObserverRow, importObserversSchema } from '../MonitoringObserversImport';
import { z } from 'zod';
import { useEffect } from 'react';

type EditProps = {
  observer: ImportObserverRow;
  updateObserver: (observer: ImportObserverRow) => void;
};

export default function EditDialog({ observer, updateObserver }: EditProps) {
  const editObserversSchema = importObserversSchema.extend({
    id: z.string(),
  });

  const form = useForm<ImportObserverRow>({
    resolver: zodResolver(editObserversSchema),
    mode: 'onChange',
    reValidateMode: 'onChange',
    defaultValues: {
      id: observer.id,
      firstName: observer.firstName,
      lastName: observer.lastName,
      email: observer.email,
      phoneNumber: observer.phoneNumber,
    },
  });

  useEffect(() => {
    form.trigger();
  }, [form.trigger]);

  function onSubmit(updatedObserver: ImportObserverRow) {
    updateObserver(updatedObserver);
  }

  return (
    <>
      <DialogHeader>
        <DialogTitle>Edit Observer</DialogTitle>
      </DialogHeader>
      <div className='py-4'>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className='grid gap-4'>
            <FormField
              control={form.control}
              name='firstName'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>First name</FormLabel>
                  <FormControl>
                    <Input type='text' {...field} {...fieldState} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name='lastName'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>Last name</FormLabel>
                  <FormControl>
                    <Input type='text' {...field} {...fieldState} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name='email'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input type='text' {...field} {...fieldState} />
                  </FormControl>
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
                  <FormControl>
                    <Input type='text' {...field} {...fieldState} />
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
