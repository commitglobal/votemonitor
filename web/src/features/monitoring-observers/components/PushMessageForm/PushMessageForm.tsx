import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { authApi } from '@/common/auth-api';
import { useQuery } from '@tanstack/react-query';
import { LevelNode } from '@/common/types';
import { useEffect, useState } from 'react';
import { useTags } from '../../queries';

function PushMessageForm() {
  const createPushMessageSchema = z.object({
    title: z.string().min(1, { message: 'Your message must have a title before sending.' }),
    messageBody: z
      .string()
      .min(1, { message: 'Your message must have a detailed description before sending.' })
      .max(1000),
    recipientsSelection: z.enum(['subgroup', 'individual']).catch('subgroup'),
    subgroupFilter: z.object({
      filterObserversBy: z.enum(['tags', 'location']).catch('location'),
      tagsFilter: z.object({
        tags: z.array(z.string()).optional(),
      }),
      locationFilter: z.object({
        location1: z.string().optional(),
        location2: z.string().optional(),
        location3: z.string().optional(),
        location4: z.string().optional(),
        location5: z.string().optional()
      }).optional()

    }),
    individualFilter: z.object({
      monitoringObserverIds: z.array(z.string()).optional()
    })
  });

  const form = useForm<z.infer<typeof createPushMessageSchema>>({
    resolver: zodResolver(createPushMessageSchema),
  });

  const watchRecipientSelection = form.watch('recipientsSelection');
  const watchFilterObserversBy = form.watch('subgroupFilter.filterObserversBy');

  useEffect(() => {
    if (watchRecipientSelection === 'individual') {
      form.register('individualFilter');
      form.unregister('subgroupFilter');
    } else {
      form.unregister('individualFilter');
      form.register('subgroupFilter');
    }
  }, [form.register, form.unregister, watchRecipientSelection]);

  useEffect(() => {
    if (watchFilterObserversBy === 'tags') {
      form.register('subgroupFilter.tagsFilter');
      form.unregister('subgroupFilter.locationFilter');
    } else {
      form.unregister('subgroupFilter.tagsFilter');
      form.register('subgroupFilter.locationFilter');
    }
  }, [form.register, form.unregister, watchFilterObserversBy]);


  const { data: nodes } = useQuery({
    queryKey: ['levels'],
    queryFn: async () => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<{ nodes: LevelNode[] }>(
        `/election-rounds/${electionRoundId}/polling-stations:fetchLevels`
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch levels');
      }

      return response.data.nodes;
    }
  });

  const { data:tags } = useTags();


  const [level1Options, setLevel1Options] = useState<LevelNode[]>([]);
  const [level2Options, setLevel2Options] = useState<LevelNode[]>([]);
  const [level3Options, setLevel3Options] = useState<LevelNode[]>([]);
  const [level4Options, setLevel4Options] = useState<LevelNode[]>([]);
  const [level5Options, setLevel5Options] = useState<LevelNode[]>([]);

  const [level1Map, setLevel1Map] = useState<{[name:string]: number}>({});
  const [level2Map, setLevel2Map] = useState<{[name:string]: number}>({});
  const [level3Map, setLevel3Map] = useState<{[name:string]: number}>({});
  const [level4Map, setLevel4Map] = useState<{[name:string]: number}>({});

  useEffect(() => {
    const level1Nodes = nodes?.filter(x => x.depth === 1) ?? [];
    const level2Nodes = nodes?.filter(x => x.depth === 2) ?? [];
    const level3Nodes = nodes?.filter(x => x.depth === 3) ?? [];
    const level4Nodes = nodes?.filter(x => x.depth === 4) ?? [];

    setLevel1Map(level1Nodes.reduce((acc: { [name: string]: number }, node) => {
      acc[node.name] = node.id;
      return acc;
    }, {}));

    setLevel2Map(level2Nodes.reduce((acc: { [name: string]: number }, node) => {
      acc[node.name] = node.id;
      return acc;
    }, {}));

    setLevel3Map(level3Nodes.reduce((acc: { [name: string]: number }, node) => {
      acc[node.name] = node.id;
      return acc;
    }, {}));

    setLevel4Map(level4Nodes.reduce((acc: { [name: string]: number }, node) => {
      acc[node.name] = node.id;
      return acc;
    }, {}));

    setLevel1Options(level1Nodes);
  }, [nodes]);

  function onSubmit(values: z.infer<typeof createPushMessageSchema>) {
    console.log(values);
  }

  return (
    <Layout title='Create new message'>
      <Card className='w-[800px] py-6'>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-8'>
              <FormField
                control={form.control}
                name='title'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel className='text-left'>
                      Title of message <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input {...field} />
                    </FormControl>
                    <FormDescription>
                      Create a short title for your message that will appear in the push notification.
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='messageBody'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel className='text-left'>
                      Message body <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Textarea rows={8} className='resize-none' {...field} />
                    </FormControl>
                    <FormDescription>1000 characters</FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='recipientsSelection'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Recipients <span className='text-red-500'>*</span>
                    </FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder='A subgroup of observers' />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value='subgroup'>A subgroup of observers</SelectItem>
                        <SelectItem value='individual'>Individual observers</SelectItem>
                      </SelectContent>
                    </Select>

                    <FormMessage />
                  </FormItem>
                )}
              />


              {watchRecipientSelection === 'individual' ?
                <>
                </> : <>
                  <FormField
                    control={form.control}
                    name='subgroupFilter.filterObserversBy'
                    render={({ field }) => (
                      <FormItem className='w-[540px]'>
                        <FormLabel>
                          Filter recipients by <span className='text-red-500'>*</span>
                        </FormLabel>
                        <Select onValueChange={field.onChange} defaultValue={field.value}>
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue placeholder='Filter observers by' />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value='tags'>Tags</SelectItem>
                            <SelectItem value='location'>Location</SelectItem>
                          </SelectContent>
                        </Select>

                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  {watchFilterObserversBy === 'tags' ? <>
                    <div>TBD</div>
                  </> :
                    <>
                      {level1Options.length > 0 && <FormField
                        control={form.control}
                        name='subgroupFilter.locationFilter.location1'
                        render={({ field }) => (
                          <FormItem className='w-[540px]'>
                            <FormLabel>
                              Location (Level 1) <span className='text-red-500'>*</span>
                            </FormLabel>
                            <Select onValueChange={(value) => {
                              field.onChange(value);
                              setLevel2Options(nodes?.filter(n => n.parentId === level1Map[value]) ?? []);
                              setLevel3Options([]);
                              setLevel4Options([]);
                              setLevel5Options([]);
                            }} defaultValue={field.value}>
                              <FormControl>
                                <SelectTrigger>
                                  <SelectValue placeholder='Location' />
                                </SelectTrigger>
                              </FormControl>
                              <SelectContent>
                                {level1Options.map(level => <SelectItem value={level.name}>{level.name}</SelectItem>)}
                              </SelectContent>
                            </Select>

                            <FormMessage />
                          </FormItem>
                        )}
                      />}

                      {level2Options.length > 0 && <FormField
                        control={form.control}
                        name='subgroupFilter.locationFilter.location2'
                        render={({ field }) => (
                          <FormItem className='w-[540px]'>
                            <FormLabel>Location (Level 2)</FormLabel>
                            <Select onValueChange={(value) => {
                              field.onChange(value);
                              setLevel3Options(nodes?.filter(n => n.parentId === level2Map[value]) ?? []);
                              setLevel4Options([]);
                              setLevel5Options([]);
                            }} defaultValue={field.value}>
                              <FormControl>
                                <SelectTrigger>
                                  <SelectValue placeholder='Location' />
                                </SelectTrigger>
                              </FormControl>
                              <SelectContent>
                                {level2Options.map(level => <SelectItem value={level.name}>{level.name}</SelectItem>)}
                              </SelectContent>
                            </Select>

                            <FormMessage />
                          </FormItem>
                        )}
                      />}

                      {level3Options.length > 0 && <FormField
                        control={form.control}
                        name='subgroupFilter.locationFilter.location3'
                        render={({ field }) => (
                          <FormItem className='w-[540px]'>
                            <FormLabel>Location (Level 3)</FormLabel>
                            <Select onValueChange={(value) => {
                              field.onChange(value);
                              setLevel4Options(nodes?.filter(n => n.parentId === level3Map[value]) ?? []);
                              setLevel5Options([]);
                            }} defaultValue={field.value}>
                              <FormControl>
                                <SelectTrigger>
                                  <SelectValue placeholder='Location' />
                                </SelectTrigger>
                              </FormControl>
                              <SelectContent>
                                {level3Options.map(level => <SelectItem value={level.name}>{level.name}</SelectItem>)}
                              </SelectContent>
                            </Select>

                            <FormMessage />
                          </FormItem>
                        )}
                      />}

                      {level4Options.length > 0 && <FormField
                        control={form.control}
                        name='subgroupFilter.locationFilter.location4'
                        render={({ field }) => (
                          <FormItem className='w-[540px]'>
                            <FormLabel>Location (Level 4)</FormLabel>
                            <Select onValueChange={(value) => {
                              field.onChange(value);
                              setLevel5Options(nodes?.filter(n => n.parentId === level4Map[value]) ?? []);
                            }} defaultValue={field.value}>
                              <FormControl>
                                <SelectTrigger>
                                  <SelectValue placeholder='Location' />
                                </SelectTrigger>
                              </FormControl>
                              <SelectContent>
                                {level4Options.map(level => <SelectItem value={level.name}>{level.name}</SelectItem>)}
                              </SelectContent>
                            </Select>

                            <FormMessage />
                          </FormItem>
                        )}
                      />}

                      {level5Options.length > 0 && <FormField
                        control={form.control}
                        name='subgroupFilter.locationFilter.location5'
                        render={({ field }) => (
                          <FormItem className='w-[540px]'>
                            <FormLabel>Location (Level 5)</FormLabel>
                            <Select onValueChange={field.onChange} defaultValue={field.value}>
                              <FormControl>
                                <SelectTrigger>
                                  <SelectValue placeholder='Location' />
                                </SelectTrigger>
                              </FormControl>
                              <SelectContent>
                                {level5Options.map(level => (<SelectItem value={level.name}>{level.name}</SelectItem>))}
                              </SelectContent>
                            </Select>

                            <FormMessage />
                          </FormItem>
                        )}
                      />}
                    </>
                  }
                </>
              }


              <div className='fixed bottom-0 left-0 bg-white py-4 px-12 flex justify-end w-screen'>
                <Button>Send message to 152 observers</Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </Layout>
  );
}

export default PushMessageForm;
