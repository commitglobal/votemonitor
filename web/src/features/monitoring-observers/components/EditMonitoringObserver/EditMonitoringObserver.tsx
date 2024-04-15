import { useLoaderData, useNavigate } from '@tanstack/react-router';
import { MonitoringObserver } from '../../models/MonitoringObserver';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { useForm } from 'react-hook-form';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Button } from '@/components/ui/button';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { TrashIcon } from '@heroicons/react/24/outline';
import { authApi } from '@/common/auth-api';
import { useMutation } from '@tanstack/react-query';
import { Tag, TagInput } from '@/components/tag/tag-input';
import { useState } from 'react';
import { v4 as uuid } from 'uuid';
import { useToast } from '@/components/ui/use-toast';

export default function EditObserver() {
  const navigate = useNavigate();
  const observer: MonitoringObserver = useLoaderData({ strict: false });
  const observerTags = observer.tags.map((tag) => ({ id: uuid(), text: tag }));
  const { toast } = useToast();

  const editObserverFormSchema = z.object({
    status: z.string(),
    tags: z.any(),
  });

  const form = useForm<z.infer<typeof editObserverFormSchema>>({
    resolver: zodResolver(editObserverFormSchema),
    defaultValues: {
      status: observer.status,
      tags: observerTags,
    },
  });

  function onSubmit(values: z.infer<typeof editObserverFormSchema>) {
    const newObj = {
      tags: values.tags.map((tag: Tag) => tag.text),
      status: values.status,
    };
    editMutation.mutate(newObj);
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
    mutationFn: (obj) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');
      const monitoringNgoId: string | null = localStorage.getItem('monitoringNgoId');

      return authApi.post<void>(
        `/election-rounds/${electionRoundId}/monitoring-ngos/${monitoringNgoId}/monitoring-observers/${observer.id}`,
        obj
      );
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Observer successfully updated',
      });
    },
  });

  const handleDelete = () => {
    deleteMutation.mutate(observer.id);
  };

  const [tags, setTags] = useState<Tag[]>(observerTags);

  const { setValue } = form;

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
          <div className='flex flex-col gap-1'>
            <p className='text-gray-700 font-bold'>Name</p>
            <p className='text-gray-900 font-normal'>{observer.name}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='text-gray-700 font-bold'>Email</p>
            <p className='text-gray-900 font-normal'>{observer.email}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='text-gray-700 font-bold'>Phone number</p>
            <p className='text-gray-900 font-normal'>{observer.phoneNumber}</p>
          </div>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='tags'
                render={({ field }) => (
                  <FormItem className='w-[540px'>
                    <FormLabel className='text-left'>Tags</FormLabel>
                    <FormControl>
                      <TagInput
                        {...field}
                        placeholder='Enter a topic'
                        tags={tags}
                        className='sm:min-w-[450px]'
                        setTags={(newTags) => {
                          console.log(newTags);
                          setTags(newTags);
                          setValue('tags', newTags as [Tag, ...Tag[]]);
                        }}
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
