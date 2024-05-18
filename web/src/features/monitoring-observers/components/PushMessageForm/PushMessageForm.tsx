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
import { useCallback, useMemo, useState } from 'react';
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
import { targetedObserversKeys, useTargetedMonitoringObservers } from '../../hooks/push-messages-queries';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { MonitoringObserverStatus } from '../../models/monitoring-observer';
import { UseQueryResult, useQuery } from '@tanstack/react-query';
import { authApi } from '@/common/auth-api';
import { buildURLSearchParams } from '@/lib/utils';
import { PageResponse, DataTableParameters } from '@/common/types';
import { TargetedMonitoringObserver } from '../../models/targeted-monitoring-observer';
import { useDebounce } from '@uidotdev/usehooks';

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
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as PushMessageTargetedObserversSearchParams;
  }, [searchText, debouncedSearch]);

  const handleDataFetchinSucceed = (pageSize: number, currentPage: number, totalCount: number) => {
    setTotalRowsCount(totalCount);
  }

  const form = useForm<z.infer<typeof createPushMessageSchema>>({
    resolver: zodResolver(createPushMessageSchema),
  });

  function onSubmit(values: z.infer<typeof createPushMessageSchema>) {
    console.log(values);
  }

  return (
    <Layout title='Create new message'>
      <Card className='py-6'>
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
              <div className='flex flex-col gap-2 items-start'>
                <div className='flex flex-col md:flex-row gap-2'>
                  <DropdownMenu>
                    <DropdownMenuTrigger>
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
                </div>
                <PollingStationsFilters className='flex flex-col md:flex-row gap-2' />
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
              )}

              <QueryParamsDataTable columns={targetedMonitoringObserverColDefs} useQuery={useTargetedMonitoringObservers} onDataFetchingSucceed={handleDataFetchinSucceed} queryParams={queryParams}/>

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
