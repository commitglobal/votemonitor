import { authApi } from '@/common/auth-api';
import Layout from '@/components/layout/Layout';
import { Tag, TagInput } from '@/components/tag/tag-input';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { useToast } from '@/components/ui/use-toast';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import { useLoaderData, useNavigate, useRouter } from '@tanstack/react-router';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { v4 as uuid } from 'uuid';
import { z } from 'zod';
import { UpdateMonitoringObserverRequest } from '../../models/monitoring-observer';
import { Route } from '@/routes/monitoring-observers/edit.$monitoringObserverId';
import { monitoringObserverDetailsQueryOptions } from '@/common/queryOptions';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import TagsSelectFormField from '@/components/ui/tag-selector';

export default function EditObserver() {
  const navigate = useNavigate();
  const router = useRouter();
  const queryClient = useQueryClient();

  const { monitoringObserverId } = Route.useParams();
  const monitoringObserverQuery = useSuspenseQuery(monitoringObserverDetailsQueryOptions(monitoringObserverId));
  const monitoringObserver = monitoringObserverQuery.data;

  const { data: availableTags } = useMonitoringObserversTags();

  const { toast } = useToast();

  const editObserverFormSchema = z.object({
    status: z.string(),
    tags: z.any(),
    firstName: z.string(),
    lastName: z.string(),
    phoneNumber: z.string(),
  });

  const form = useForm<z.infer<typeof editObserverFormSchema>>({
    resolver: zodResolver(editObserverFormSchema),
    defaultValues: {
      status: monitoringObserver.status,
      tags: monitoringObserver.tags,
      firstName: monitoringObserver.firstName,
      lastName: monitoringObserver.lastName,
      phoneNumber: monitoringObserver.phoneNumber,
    },
  });

  function onSubmit(values: z.infer<typeof editObserverFormSchema>) {
    const newObj: UpdateMonitoringObserverRequest = {
      tags: values.tags,
      status: values.status,
      firstName: values.firstName,
      lastName: values.lastName,
      phoneNumber: values.phoneNumber,
    };

    editMutation.mutate(newObj);
  }

  const editMutation = useMutation({
    mutationFn: (obj: UpdateMonitoringObserverRequest) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      return authApi.post<void>(
        `/election-rounds/${electionRoundId}/monitoring-observers/${monitoringObserver.id}`,
        obj
      );
    },
    mutationKey: ['monitoring-observers'],
    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Observer successfully updated',
      });
      router.invalidate();
      queryClient.invalidateQueries({ queryKey: ['monitoring-observers'] });
      queryClient.invalidateQueries({ queryKey: ['tags'] });

      navigate({ to: '/monitoring-observers/view/$monitoringObserverId/$tab', params: { monitoringObserverId: monitoringObserver.id, tab: 'details' } })
    },
  });

  return (
    <Layout title={`Edit ${monitoringObserver.firstName} ${monitoringObserver.lastName}`}>
      <Card className='w-[800px] pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <div className='flex flex-row justify-between items-center'>
            <CardTitle className='text-xl'>Edit observer</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <div className='flex flex-col gap-1'>
            <p className='text-gray-700 font-bold'>Email</p>
            <p className='text-gray-900 font-normal'>{monitoringObserver.email}</p>
          </div>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='firstName'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className='text-left'>First name</FormLabel>
                    <FormControl>
                      <Input {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='lastName'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel className='text-left'>Last name</FormLabel>
                    <FormControl>
                      <Input {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='phoneNumber'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className='text-left'>Phone number</FormLabel>
                    <FormControl>
                      <Input {...field} />
                    </FormControl>
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
                        options={availableTags?.filter(tag => !field.value.includes(tag)) ?? []}
                        defaultValue={field.value}
                        onValueChange={field.onChange}
                        placeholder="Observer tags"
                      />
                    </FormControl>
                    <FormMessage />
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
                <div className='flex gap-2'>
                  <Button
                    variant='outline'
                    type='button'
                    onClick={() => { void navigate({ to: '/monitoring-observers/view/$monitoringObserverId/$tab', params: { monitoringObserverId: monitoringObserver.id, tab: 'details' } }) }}>
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
