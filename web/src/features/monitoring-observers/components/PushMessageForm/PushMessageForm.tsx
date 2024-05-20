import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { zodResolver } from '@hookform/resolvers/zod';
import { ChangeEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { useMonitoringObserversTags } from '../../../../hooks/tags-queries';
import { targetedMonitoringObserverColDefs } from '../../utils/column-defs';
import { useNavigate } from '@tanstack/react-router';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Route } from '@/routes/monitoring-observers/create-new-message';
import { ChevronDownIcon } from '@heroicons/react/24/outline';
import { PushMessageTargetedObserversSearchParams } from '../../models/search-params';
import { FilterBadge } from '@/components/ui/badge';
import { useTargetedMonitoringObservers } from '../../hooks/push-messages-queries';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { MonitoringObserverStatus } from '../../models/monitoring-observer';
import { useDebounce } from '@uidotdev/usehooks';
import { toast } from '@/components/ui/use-toast';
import { useMutation } from '@tanstack/react-query';
import { authApi } from '@/common/auth-api';
import { SendPushNotificationRequest } from '../../models/push-message';

const createPushMessageSchema = z.object({
  title: z.string().min(1, { message: 'Your message must have a title before sending.' }),
  messageBody: z
    .string()
    .min(1, { message: 'Your message must have a detailed description before sending.' })
    .max(1000)
});

function PushMessageForm() {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const [totalRowsCount, setTotalRowsCount] = useState(0);
  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);

  const { data: tags } = useMonitoringObserversTags();

  const onTagsFilterChange = useCallback(
    (tag: string) => () => {
      void navigate({
        search: (prev: PushMessageTargetedObserversSearchParams) => {
          const prevTagsFilter = prev.tagsFilter ?? [];
          const newTags = prevTagsFilter.includes(tag)
            ? prevTagsFilter.filter((t) => t !== tag)
            : [...prevTagsFilter, tag];

          return { ...prev, tagsFilter: newTags.length > 0 ? newTags : undefined };
        },
      });
    },
    [navigate]
  );

  const onClearFilter = useCallback(
    (filter: keyof PushMessageTargetedObserversSearchParams | (keyof PushMessageTargetedObserversSearchParams)[]) => () => {
      const filters = Array.isArray(filter)
        ? Object.fromEntries(filter.map((key) => [key, undefined]))
        : { [filter]: undefined };
      void navigate({ search: (prev) => ({ ...prev, ...filters }) });
    },
    [navigate]
  );

  useEffect(() => {
    navigate({ search: (prev) => ({ ...prev, searchText: debouncedSearchText }) })
  }, [debouncedSearchText]);

  const debouncedSearch = useDebounce(search, 300,);

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', debouncedSearchText],
      ['statusFilter', debouncedSearch.statusFilter],
      ['tagsFilter', debouncedSearch.tagsFilter],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as PushMessageTargetedObserversSearchParams;

  }, [debouncedSearchText, debouncedSearch]);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  const handleDataFetchinSucceed = (pageSize: number, currentPage: number, totalCount: number) => {
    setTotalRowsCount(totalCount);
  }

  const form = useForm<z.infer<typeof createPushMessageSchema>>({
    resolver: zodResolver(createPushMessageSchema),
    defaultValues: {
      title: '',
      messageBody: ''
    }
  });

  const sendNotificationMutation = useMutation({
    mutationFn: (obj:SendPushNotificationRequest) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      return authApi.post<SendPushNotificationRequest>(
        `/election-rounds/${electionRoundId}/notifications:send`,
        obj
      );
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Notification sent',
      });
    },
  });

  function onSubmit(values: z.infer<typeof createPushMessageSchema>) {
    sendNotificationMutation.mutate({
      title: values.title,
      body: values.messageBody,
      ...queryParams
    })
  }

  return (
    <Layout title='Create new message'>
      <Card className='py-6'>
        <CardContent >
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)}>
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

              <div className='grid grid-cols-6 gap-4 items-center mb-4'>
                <Input onChange={handleSearchInput} placeholder='Search' />

                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button className='border-gray-200 gap-1 hover:bg-white w-[180px]' variant='outline'>
                      <span className='text-sm font-normal text-slate-700'>Observer tags</span>
                      {search.tagsFilter && (
                        <span className='bg-purple-50 text-purple-600 rounded-full inline-block px-2'>
                          {search.tagsFilter.length}
                        </span>
                      )}
                      <ChevronDownIcon className='w-[20px] ml-auto' />
                    </Button>
                  </DropdownMenuTrigger>

                  <DropdownMenuContent>
                    {tags?.map((tag) => (
                      <DropdownMenuCheckboxItem
                        checked={search.tagsFilter?.includes(tag.text)}
                        onCheckedChange={onTagsFilterChange(tag.text)}
                        key={tag.id}>
                        {tag.text}
                      </DropdownMenuCheckboxItem>
                    ))}
                  </DropdownMenuContent>
                </DropdownMenu>

                <Select
                  onValueChange={(value) => {
                    void navigate({ search: (prev) => ({ ...prev, statusFilter: value }) });
                  }}
                  value={search.statusFilter ?? ''}>
                  <SelectTrigger className="w-[180px]">
                    <SelectValue placeholder='Observer status' />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectGroup>
                      {Object.values(MonitoringObserverStatus).map((value) => (
                        <SelectItem value={value} key={value}>{value}</SelectItem>
                      ))}
                    </SelectGroup>
                  </SelectContent>
                </Select>
                <PollingStationsFilters />

                <Button
                  type='button'
                  onClick={() => {
                    void navigate({});
                  }}
                  variant='ghost-primary'
                  className="w-[180px]">
                  Reset filters
                </Button>
              </div>

              {Object.entries(search).length > 0 && (
                <div className='col-span-full flex gap-2 flex-wrap'>
                  {search.tagsFilter?.map((tag) => (
                    <FilterBadge label={`Observer tags: ${tag}`} onClear={onTagsFilterChange(tag)} />
                  ))}


                  {search.level1Filter && (
                    <FilterBadge
                      label={`Location - L1: ${search.level1Filter}`}
                      onClear={onClearFilter(['level1Filter', 'level2Filter', 'level3Filter', 'level4Filter', 'level5Filter'])}
                    />
                  )}

                  {search.level2Filter && (
                    <FilterBadge
                      label={`Location - L2: ${search.level2Filter}`}
                      onClear={onClearFilter(['level2Filter', 'level3Filter', 'level4Filter', 'level5Filter'])}
                    />
                  )}

                  {search.level3Filter && (
                    <FilterBadge
                      label={`Location - L3: ${search.level3Filter}`}
                      onClear={onClearFilter(['level3Filter', 'level4Filter', 'level5Filter'])}
                    />
                  )}

                  {search.level4Filter && (
                    <FilterBadge
                      label={`Location - L4: ${search.level4Filter}`}
                      onClear={onClearFilter(['level4Filter', 'level5Filter'])}
                    />
                  )}

                  {search.level5Filter && (
                    <FilterBadge label={`Location - L5: ${search.level5Filter}`} onClear={onClearFilter('level5Filter')} />
                  )}
                </div>
              )}

              <QueryParamsDataTable columns={targetedMonitoringObserverColDefs} useQuery={useTargetedMonitoringObservers} onDataFetchingSucceed={handleDataFetchinSucceed} queryParams={queryParams} />

              <div className='fixed bottom-0 left-0 bg-white py-4 px-12 flex justify-end w-screen'>
                <Button>Send message to {totalRowsCount} observers</Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </Layout>
  );
}

export default PushMessageForm;
