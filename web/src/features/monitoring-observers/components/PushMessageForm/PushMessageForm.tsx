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
import { zodResolver } from '@hookform/resolvers/zod';
import { type ChangeEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { authApi } from '@/common/auth-api';
import type { FunctionComponent } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { RichTextEditor } from '@/components/rich-text-editor';
import { FilterBadge } from '@/components/ui/badge';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { Route } from '@/routes/monitoring-observers/create-new-message';
import { ChevronDownIcon } from '@heroicons/react/24/outline';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useMonitoringObserversTags } from '../../../../hooks/tags-queries';
import { pushMessagesKeys, useTargetedMonitoringObservers } from '../../hooks/push-messages-queries';
import { MonitoringObserverStatus } from '../../models/monitoring-observer';
import type { SendPushNotificationRequest } from '../../models/push-message';
import type { PushMessageTargetedObserversSearchParams } from '../../models/search-params';
import { targetedMonitoringObserverColDefs } from '../../utils/column-defs';

const createPushMessageSchema = z.object({
  title: z.string().min(1, { message: 'Your message must have a title before sending.' }),
  messageBody: z
    .string()
    .min(1, { message: 'Your message must have a detailed description before sending.' })
    .max(1000),
});

function PushMessageForm(): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const [totalRowsCount, setTotalRowsCount] = useState(0);
  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: tags } = useMonitoringObserversTags(currentElectionRoundId);
  const router = useRouter();

  const queryClient = useQueryClient();

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
    (filter: keyof PushMessageTargetedObserversSearchParams | (keyof PushMessageTargetedObserversSearchParams)[]) =>
      () => {
        const filters = Array.isArray(filter)
          ? Object.fromEntries(filter.map((key) => [key, undefined]))
          : { [filter]: undefined };
        void navigate({ search: (prev) => ({ ...prev, ...filters }) });
      },
    [navigate]
  );

  useEffect(() => {
    void navigate({ search: (prev) => ({ ...prev, searchText: debouncedSearchText }) });
  }, [debouncedSearchText]);

  const debouncedSearch = useDebounce(search, 300);

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
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as PushMessageTargetedObserversSearchParams;
  }, [debouncedSearchText, debouncedSearch]);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  const handleDataFetchingSucceed = (pageSize: number, currentPage: number, totalCount: number): void => {
    setTotalRowsCount(totalCount);
  };

  const form = useForm<z.infer<typeof createPushMessageSchema>>({
    resolver: zodResolver(createPushMessageSchema),
    defaultValues: {
      title: '',
      messageBody: '',
    },
  });

  const sendNotificationMutation = useMutation({
    mutationFn: ({ electionRoundId, request }: { electionRoundId: string; request: SendPushNotificationRequest }) => {
      return authApi.post<SendPushNotificationRequest>(
        `/election-rounds/${electionRoundId}/notifications:send`,
        request
      );
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: pushMessagesKeys.all(currentElectionRoundId) });
      toast({
        title: 'Success',
        description: 'Notification sent',
      });

      router.invalidate();
      navigate({ to: '/monitoring-observers/$tab', params: { tab: 'push-messages' } });
    },
  });

  function onSubmit(values: z.infer<typeof createPushMessageSchema>): void {
    sendNotificationMutation.mutate({
      electionRoundId: currentElectionRoundId,
      request: {
        title: values.title,
        body: values.messageBody,
        ...queryParams,
      },
    });
  }

  return (
    <Layout title='Create new message'>
      <Card className='py-6'>
        <CardContent>
          <Form {...form}>
            {/* eslint-disable-next-line @typescript-eslint/no-misused-promises */}
            <form onSubmit={form.handleSubmit(onSubmit)}>
              <h2 className='mb-10 text-xl font-medium'>1. Compose your message</h2>

              <div className='flex flex-col gap-6 mb-20'>
                <FormField
                  control={form.control}
                  name='title'
                  render={({ field }) => (
                    <FormItem className='w-[540px]'>
                      <FormLabel className='text-left'>
                        Title of message <span className='text-red-500'>*</span>
                      </FormLabel>
                      <FormControl>
                        <Input {...field} maxLength={256} />
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
                        <RichTextEditor {...field} onValueChange={field.onChange} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <h2 className='mb-10 text-xl font-medium'>
                2. Filter the list of observers to reduce it to the desired recipients of the message
              </h2>

              <div className='flex flex-col gap-6'>
                <div className='grid items-center grid-cols-6 gap-4'>
                  <Input onChange={handleSearchInput} placeholder='Search' />

                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button className='border-gray-200 gap-1 hover:bg-white w-[180px]' variant='outline'>
                        <span className='text-sm font-normal text-slate-700'>Observer tags</span>
                        {search.tagsFilter && (
                          <span className='inline-block px-2 text-purple-600 rounded-full bg-purple-50'>
                            {search.tagsFilter.length}
                          </span>
                        )}
                        <ChevronDownIcon className='w-[20px] ml-auto' />
                      </Button>
                    </DropdownMenuTrigger>

                    <DropdownMenuContent>
                      {tags?.map((tag) => (
                        <DropdownMenuCheckboxItem
                          checked={search.tagsFilter?.includes(tag)}
                          onCheckedChange={onTagsFilterChange(tag)}
                          key={tag}>
                          {tag}
                        </DropdownMenuCheckboxItem>
                      ))}
                    </DropdownMenuContent>
                  </DropdownMenu>

                  <Select
                    onValueChange={(value) => {
                      void navigate({ search: (prev) => ({ ...prev, statusFilter: value }) });
                    }}
                    value={search.statusFilter ?? ''}>
                    <SelectTrigger className='w-[180px]'>
                      <SelectValue placeholder='Observer status' />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectGroup>
                        {Object.values(MonitoringObserverStatus).map((value) => (
                          <SelectItem value={value} key={value}>
                            {value}
                          </SelectItem>
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
                    className='w-[180px]'>
                    Reset filters
                  </Button>
                </div>

                {Object.entries(search).length > 0 && (
                  <div className='flex flex-wrap gap-2 col-span-full'>
                    {search.tagsFilter?.map((tag) => (
                      <FilterBadge label={`Observer tags: ${tag}`} onClear={onTagsFilterChange(tag)} />
                    ))}

                    {search.statusFilter && (
                      <FilterBadge label={`Status: ${search.statusFilter}`} onClear={onClearFilter('statusFilter')} />
                    )}

                    {search.level1Filter && (
                      <FilterBadge
                        label={`Location - L1: ${search.level1Filter}`}
                        onClear={onClearFilter([
                          'level1Filter',
                          'level2Filter',
                          'level3Filter',
                          'level4Filter',
                          'level5Filter',
                          'pollingStationNumberFilter',
                        ])}
                      />
                    )}

                    {search.level2Filter && (
                      <FilterBadge
                        label={`Location - L2: ${search.level2Filter}`}
                        onClear={onClearFilter([
                          'level2Filter',
                          'level3Filter',
                          'level4Filter',
                          'level5Filter',
                          'pollingStationNumberFilter',
                        ])}
                      />
                    )}

                    {search.level3Filter && (
                      <FilterBadge
                        label={`Location - L3: ${search.level3Filter}`}
                        onClear={onClearFilter([
                          'level3Filter',
                          'level4Filter',
                          'level5Filter',
                          'pollingStationNumberFilter',
                        ])}
                      />
                    )}

                    {search.level4Filter && (
                      <FilterBadge
                        label={`Location - L4: ${search.level4Filter}`}
                        onClear={onClearFilter(['level4Filter', 'level5Filter', 'pollingStationNumberFilter'])}
                      />
                    )}

                    {search.level5Filter && (
                      <FilterBadge
                        label={`Location - L5: ${search.level5Filter}`}
                        onClear={onClearFilter(['level5Filter', 'pollingStationNumberFilter'])}
                      />
                    )}

                    {search.pollingStationNumberFilter && (
                      <FilterBadge
                        label={`PS Number: ${search.pollingStationNumberFilter}`}
                        onClear={onClearFilter('pollingStationNumberFilter')}
                      />
                    )}
                  </div>
                )}

                <QueryParamsDataTable
                  columns={targetedMonitoringObserverColDefs}
                  useQuery={(params) => useTargetedMonitoringObservers(currentElectionRoundId, params)}
                  onDataFetchingSucceed={handleDataFetchingSucceed}
                  queryParams={queryParams}
                />
              </div>

              <div className='fixed bottom-0 left-0 flex justify-end w-screen px-12 py-4 bg-white'>
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
