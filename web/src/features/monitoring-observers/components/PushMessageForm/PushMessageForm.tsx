import { authApi } from '@/common/auth-api';
import { DataTableParameters, PageResponse } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { DataTable } from '@/components/ui/DataTable/DataTable';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { buildURLSearchParams } from '@/lib/utils';
import { zodResolver } from '@hookform/resolvers/zod';
import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { X } from 'lucide-react';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { MonitoringObserver } from '../../models/MonitoringObserver';
import { useMonitoringObserversTags } from '../../queries';
import { targetedMonitoringObserverColDefs } from '../../utils/column-defs';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';

const createPushMessageSchema = z.object({
  title: z.string().min(1, { message: 'Your message must have a title before sending.' }),
  messageBody: z
    .string()
    .min(1, { message: 'Your message must have a detailed description before sending.' })
    .max(1000),

  location1: z.string().optional().catch(''),
  location2: z.string().optional().catch(''),
  location3: z.string().optional().catch(''),
  location4: z.string().optional().catch(''),
  location5: z.string().optional().catch(''),
  tags: z.array(z.string()).optional().catch([]),
});

type ListTargetedMonitoringObserverResponse = PageResponse<MonitoringObserver>;

type UseTargetedMonitoringObserversResult = UseQueryResult<ListTargetedMonitoringObserverResponse, Error>;

function PushMessageForm() {
  const [tagsFilter, setTagsFilter] = useState<string[]>([]);
  const [searchText, setSearchText] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const { data: tags } = useMonitoringObserversTags();

  const useTargetedMonitoringObservers = (p: DataTableParameters): UseTargetedMonitoringObserversResult => {
    return useQuery({
      queryKey: [
        'monitoring-observers',
        p.pageNumber,
        p.pageSize,
        p.sortColumnName,
        p.sortOrder,
        searchText,
        statusFilter,
        tagsFilter,
      ],
      queryFn: async () => {
        const params: any = {
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          SortColumnName: p.sortColumnName,
          SortOrder: p.sortOrder,
          nameFilter: searchText,
          statusFilter,
          tagsFilter
        };

        const electionRoundId: string | null = localStorage.getItem('electionRoundId');
        const searchParams = buildURLSearchParams(params);

        const response = await authApi
          .get<PageResponse<MonitoringObserver>>(`election-rounds/${electionRoundId}/notifications:listRecipients`,
            {
              params: searchParams,
            }
          );

        if (response.status !== 200) {
          throw new Error('Failed to fetch monitoring observers');
        }

        return response.data;
      },
    });
  };

  const handleStatusFilter = (status: string) => {
    setStatusFilter(status);
  };

  const resetFilters = () => {
    setStatusFilter('');
    setTagsFilter([]);
  };

  const toggleTagsFilter = (tag: string) => {
    setTagsFilter((prevTags: any) => {
      if (!prevTags.includes(tag)) {
        return [...prevTags, tag];
      } else {
        return prevTags.filter((tagText: string) => tagText !== tag);
      }
    });
  };



  const form = useForm<z.infer<typeof createPushMessageSchema>>({
    resolver: zodResolver(createPushMessageSchema),
  });


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

              <div className='table-filters flex flex-row gap-4 items-center'>
                <Select value={statusFilter} onValueChange={handleStatusFilter}>
                  <SelectTrigger className='w-[180px]'>
                    <SelectValue placeholder='Observer status' />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectGroup>
                      <SelectItem value='Active'>Active</SelectItem>
                      <SelectItem value='Pending'>Pending</SelectItem>
                      <SelectItem value='Suspended'>Suspended</SelectItem>
                    </SelectGroup>
                  </SelectContent>
                </Select>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <div className='flex h-10 w-48 rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50'>
                      Tags
                    </div>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent className='w-56'>
                    {tags?.map((tag) => (
                      <DropdownMenuCheckboxItem
                        checked={tagsFilter.includes(tag.text)}
                        onCheckedChange={() => toggleTagsFilter(tag.text)}
                        key={tag.id}>
                        {tag.text}
                      </DropdownMenuCheckboxItem>
                    ))}
                  </DropdownMenuContent>
                </DropdownMenu>
                <PollingStationsFilters />

                <Button
                  onClick={() => {
                    void navigate({});
                  }}
                  variant='ghost-primary'>
                  Reset filters
                </Button>

                <div className='flex flex-row gap-2 flex-wrap'>

                  {tagsFilter.map((tag) => (
                    <span
                      key={tag}
                      onClick={() => toggleTagsFilter(tag)}
                      className='rounded-full cursor-pointer py-1 px-4 bg-purple-100 text-sm text-purple-900 font-medium flex items-center gap-2'>
                      Tags: {tag}
                      <X size={14} />
                    </span>
                  ))}
                </div>
              </div>

              <DataTable columns={targetedMonitoringObserverColDefs} useQuery={useTargetedMonitoringObservers} />

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
