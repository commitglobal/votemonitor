import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect, useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { z } from 'zod';

import type { FunctionComponent } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { RichTextEditor } from '@/components/rich-text-editor';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsFollowUpFilter } from '@/features/filtering/components/FormSubmissionsFollowUpFilter';
import { FormSubmissionsFormFilter } from '@/features/filtering/components/FormSubmissionsFormFilter';
import { FormSubmissionsFromDateFilter } from '@/features/filtering/components/FormSubmissionsFromDateFilter';
import { FormSubmissionsQuestionsAnsweredFilter } from '@/features/filtering/components/FormSubmissionsQuestionsAnsweredFilter';
import { FormSubmissionsToDateFilter } from '@/features/filtering/components/FormSubmissionsToDateFilter';
import { FormTypeFilter } from '@/features/filtering/components/FormTypeFilter';
import { HasQuickReportsFilter } from '@/features/filtering/components/HasQuickReportsFilter';
import { QuickReportsFollowUpFilter } from '@/features/filtering/components/QuickReportsFollowUpFilter';
import { QuickReportsIncidentCategoryFilter } from '@/features/filtering/components/QuickReportsIncidentCategoryFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { toBoolean } from '@/lib/utils';
import { Route } from '@/routes/(app)/monitoring-observers/create-new-message';
import API from '@/services/api';
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
  messageBody: z.string().min(1, { message: 'Your message must have a detailed description before sending.' }),
});

function PushMessageForm(): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const [totalRowsCount, setTotalRowsCount] = useState(0);
  const [searchText, setSearchText] = useState<string>(search.searchText ?? '');
  const debouncedSearchText = useDebounce(searchText, 300);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const router = useRouter();

  const queryClient = useQueryClient();

  useEffect(() => {
    navigate({
      to: '.',
      replace: true,
      search: (prev: any) => ({ ...prev, [FILTER_KEY.SearchText]: debouncedSearchText }),
    });
  }, [debouncedSearchText]);

  useEffect(() => {
    setSearchText(search.searchText ?? '');
  }, [search.searchText]);

  const debouncedSearch = useDebounce(search, 300);

  const queryParams = useMemo(() => {
    const params: SendPushNotificationRequest = {
      searchText: searchText,
      statusFilter: debouncedSearch.statusFilter,
      formTypeFilter: debouncedSearch.formTypeFilter,
      level1Filter: debouncedSearch.level1Filter,
      level2Filter: debouncedSearch.level2Filter,
      level3Filter: debouncedSearch.level3Filter,
      level4Filter: debouncedSearch.level4Filter,
      level5Filter: debouncedSearch.level5Filter,
      pollingStationNumberFilter: debouncedSearch.pollingStationNumberFilter,
      followUpStatus: debouncedSearch.followUpStatus,
      questionsAnswered: debouncedSearch.questionsAnswered,
      hasFlaggedAnswers: toBoolean(debouncedSearch.hasFlaggedAnswers),
      hasNotes: toBoolean(debouncedSearch.hasNotes),
      hasAttachments: toBoolean(debouncedSearch.hasAttachments),
      hasQuickReports: toBoolean(debouncedSearch.hasQuickReports),
      tagsFilter: debouncedSearch.tagsFilter,
      formId: debouncedSearch.formId,
      fromDateFilter: debouncedSearch.submissionsFromDate?.toISOString(),
      toDateFilter: debouncedSearch.submissionsToDate?.toISOString(),
      monitoringObserverStatus: debouncedSearch.monitoringObserverStatus,
      quickReportIncidentCategory: debouncedSearch.incidentCategory,
      quickReportFollowUpStatus: debouncedSearch.quickReportFollowUpStatus,
    };

    return params;
  }, [debouncedSearchText, debouncedSearch]);

  const handleDataFetchingSucceed = (pageSize: number, currentPage: number, totalCount: number): void => {
    setTotalRowsCount(totalCount);
  };

  const form = useForm<z.infer<typeof createPushMessageSchema>>({
    resolver: zodResolver(createPushMessageSchema),
    mode: 'all',
    defaultValues: {
      title: '',
      messageBody: '',
    },
  });

  const sendNotificationMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      request,
    }: {
      electionRoundId: string;
      request: SendPushNotificationRequest & { title: string; body: string };
    }) => {
      return API.post<PushMessageTargetedObserversSearchParams>(
        `/election-rounds/${electionRoundId}/notifications:send`,
        request
      );
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: pushMessagesKeys.all(currentElectionRoundId) });
      toast.success('Notification sent');

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

              <div className='mb-8'>
                <FilteringContainer>
                  <Input
                    onChange={(ev) => setSearchText(ev.currentTarget.value)}
                    value={searchText}
                    placeholder='Search'
                  />
                  <MonitoringObserverTagsSelect isUsingAlternativeFilteringKey />
                  <MonitoringObserverStatusSelect />
                  <FormTypeFilter />
                  <FormSubmissionsFormFilter />
                  <FormSubmissionsQuestionsAnsweredFilter />
                  <FormSubmissionsFollowUpFilter />

                  <FormSubmissionsFromDateFilter />
                  <FormSubmissionsToDateFilter />
                  <HasQuickReportsFilter />
                  <QuickReportsIncidentCategoryFilter />
                  <QuickReportsFollowUpFilter placeholder='Quick reports follow up status' />

                  <PollingStationsFilters />
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
