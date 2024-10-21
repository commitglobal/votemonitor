import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { zodResolver } from '@hookform/resolvers/zod';
import { type ChangeEvent, useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { authApi } from '@/common/auth-api';
import type { FunctionComponent } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Textarea } from '@/components/ui/textarea';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormTypeSelect } from '@/features/forms/components/filtering/FormTypeSelect';
import { FormSubmissionsFormSelect } from '@/features/responses/filtering/FormSubmissionsFormSelect';
import { FormSubmissionsFromDateFilter } from '@/features/responses/filtering/FormSubmissionsFromDateFilter';
import { FormSubmissionsQuestionsAnsweredSelect } from '@/features/responses/filtering/FormSubmissionsQuestionsAnsweredSelect';
import { FormSubmissionsToDateFilter } from '@/features/responses/filtering/FormSubmissionsToDateFilter';
import { Route } from '@/routes/monitoring-observers/create-new-message';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { MonitoringObserverStatusSelect } from '../../filtering/MonitoringObserverStatusSelect';
import { MonitoringObserverTagsSelect } from '../../filtering/MonitoringObserverTagsSelect';
import { pushMessagesKeys, useTargetedMonitoringObservers } from '../../hooks/push-messages-queries';
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
  const router = useRouter();

  const queryClient = useQueryClient();

  useEffect(() => {
    void navigate({ search: (prev) => ({ ...prev, searchText: debouncedSearchText }) });
  }, [debouncedSearchText]);

  const debouncedSearch = useDebounce(search, 300);

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', debouncedSearchText],
      ['statusFilter', debouncedSearch.monitoringObserverStatus],
      ['tagsFilter', debouncedSearch.tags],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['formTypeFilter', debouncedSearch.formTypeFilter],
      ['questionsAnswered', debouncedSearch.questionsAnswered],
      ['formId', debouncedSearch.formId],
      ['fromDateFilter', debouncedSearch.submissionsFromDate?.toISOString()],
      ['toDateFilter', debouncedSearch.submissionsToDate?.toISOString()],
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
                  render={({ field, fieldState }) => (
                    <FormItem className='w-[540px]'>
                      <FormLabel className='text-left'>
                        Message body <span className='text-red-500'>*</span>
                      </FormLabel>
                      <FormControl>
                        {/* <RichTextEditor {...field} onValueChange={field.onChange} /> */}
                        <Textarea {...field} {...fieldState} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <h2 className='mb-10 text-xl font-medium'>
                2. Filter the list of observers to reduce it to the desired recipients of the message
              </h2>

              <div className='mb-8'>
                <FilteringContainer>
                  <Input onChange={handleSearchInput} placeholder='Search' />
                  <MonitoringObserverTagsSelect />
                  <MonitoringObserverStatusSelect />
                  <FormTypeSelect />
                  <FormSubmissionsFormSelect />
                  <FormSubmissionsQuestionsAnsweredSelect />
                  <PollingStationsFilters />
                  <FormSubmissionsFromDateFilter />
                  <FormSubmissionsToDateFilter />
                </FilteringContainer>
              </div>

              <QueryParamsDataTable
                columns={targetedMonitoringObserverColDefs}
                useQuery={(params) => useTargetedMonitoringObservers(currentElectionRoundId, params)}
                onDataFetchingSucceed={handleDataFetchingSucceed}
                queryParams={queryParams}
              />

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
