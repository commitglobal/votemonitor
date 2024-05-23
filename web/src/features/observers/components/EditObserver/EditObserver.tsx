import { authApi } from '@/common/auth-api';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { observerDetailsQueryOptions } from '@/routes/observers/$observerId';
import { Route } from '@/routes/observers_.$observerId.edit';
import { TrashIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

export default function EditObserver() {
  const navigate = useNavigate();
  const { observerId } = Route.useParams();
  const observerQuery = useSuspenseQuery(observerDetailsQueryOptions(observerId));
  const observer = observerQuery.data;

  const editObserverFormSchema = z.object({
    name: z.string().min(2, {
      message: 'This field is mandatory',
    }),
    login: z.string().min(1, { message: 'This field is mandatory' }).email('Email is not valid'),
    phoneNumber: z
      .string()
      .min(1, { message: 'This field is required' }),
    status: z.string(),
  });

  const form = useForm<z.infer<typeof editObserverFormSchema>>({
    resolver: zodResolver(editObserverFormSchema),
    defaultValues: {
      name: observer.name,
      login: observer.email,
      phoneNumber: observer.phoneNumber,
      status: observer.status,
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
    <Layout title={`Edit ${observer.name}`}>
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
                name='name'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Name' {...field} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='login'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Email <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Email address' {...field} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='phoneNumber'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Phone number <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Phone number' {...field} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='status'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Status <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Select onValueChange={field.onChange} defaultValue={field.value} value={field.value}>
                        <SelectTrigger>
                          <SelectValue placeholder='Observer status' />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectGroup>
                            <SelectItem value='Active'>Active</SelectItem>
                            <SelectItem value='Suspended'>Suspended</SelectItem>
                          </SelectGroup>
                        </SelectContent>
                      </Select>
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
        <CardFooter className='flex justify-between'></CardFooter>
      </Card>
    </Layout>
  );
}
