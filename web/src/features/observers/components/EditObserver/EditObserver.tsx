import { authApi } from '@/common/auth-api';
import Layout from '@/components/layout/Layout';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { observerDetailsQueryOptions } from '@/routes/observers/$observerId';
import { Route } from '@/routes/observers_.$observerId.edit';
import { TrashIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useBlocker, useNavigate } from '@tanstack/react-router';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

export default function EditObserver() {
  const navigate = useNavigate();
  const { observerId } = Route.useParams();
  const observerQuery = useSuspenseQuery(observerDetailsQueryOptions(observerId));
  const observer = observerQuery.data;
  const confirm = useConfirm();

  const editObserverFormSchema = z.object({
    lastName: z.string().min(1, {
      message: 'Last name must be at least 1 characters long',
    }),
    firstName: z.string().min(1, {
      message: 'First name must be at least 1 characters long',
    }),
    email: z.string().min(1, { message: 'Email is required' }).email('Please enter a valid email address'),
    phoneNumber: z.string(),
  });

  const form = useForm<z.infer<typeof editObserverFormSchema>>({
    resolver: zodResolver(editObserverFormSchema),
    mode: 'all',
    defaultValues: {
      firstName: observer.firstName,
      lastName: observer.lastName,
      email: observer.email,
      phoneNumber: observer.phoneNumber,
    },
  });

  useBlocker({
    shouldBlockFn: async () => {
      if (!form.formState.isDirty) {
        return false;
      }

      return !(await confirm({
        title: `Unsaved Changes Detected`,
        body: 'You have unsaved changes. If you leave this page, your changes will be lost. Are you sure you want to continue?',
        actionButton: 'Leave',
        cancelButton: 'Stay',
      }));
    },
  });

  function onSubmit(values: z.infer<typeof editObserverFormSchema>) {
    editMutation.mutate({
      observerId: observer.id,
      obj: values,
    });
  }

  const deleteMutation = useMutation({
    mutationFn: (observerId: string) => {
      return authApi.delete<void>(`/observers/${observerId}`);
    },
    onSuccess: () => {
      navigate({ to: '/observers' });
    },
  });

  const editMutation = useMutation({
    mutationFn: ({ observerId, obj }: any) => {
      return authApi.put<void>(`/observers/${observerId}`, obj);
    },
  });

  const handleDelete = () => {
    deleteMutation.mutate(observer.id);
  };

  return (
    <Layout title={`Edit ${observer.firstName + ' ' + observer.lastName}`}>
      <Card className='w-[800px] pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <div className='flex flex-row justify-between items-center'>
            <CardTitle className='text-xl'>Edit observer</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='firstName'
                render={({ field, fieldState }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='First name' {...field} {...fieldState} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='lastName'
                render={({ field, fieldState }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Last name' {...field} {...fieldState} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='email'
                render={({ field, fieldState }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Email <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Email address' {...field} {...fieldState} type='email' />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='phoneNumber'
                render={({ field, fieldState }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Phone number <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Phone number' {...field} {...fieldState} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />

              <div className='flex justify-between'>
                <Button
                  onClick={handleDelete}
                  variant='ghost'
                  className='text-destructive hover:text-destructive hover:bg-background px-0'>
                  <TrashIcon className='w-[18px] mr-2' />
                  Delete observer
                </Button>
                <div className='flex gap-2'>
                  <Button variant='outline' type='submit'>
                    Cancel
                  </Button>
                  <Button type='submit' className='px-6'>
                    Save
                  </Button>
                </div>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </Layout>
  );
}
