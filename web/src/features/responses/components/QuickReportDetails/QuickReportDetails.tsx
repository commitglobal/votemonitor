import { authApi } from '@/common/auth-api';
import { DateTimeFormat } from '@/common/formats';
import { usePrevSearch } from '@/common/prev-search-store';
import { QuickReportFollowUpStatus, type FunctionComponent, ElectionRoundStatus } from '@/common/types';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { queryClient } from '@/main';
import { Route, quickReportDetailsQueryOptions } from '@/routes/responses/quick-reports/$quickReportId';
import { ArrowTopRightOnSquareIcon } from '@heroicons/react/24/outline';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { Link, useRouter } from '@tanstack/react-router';
import { format } from 'date-fns';
import { quickReportKeys } from '../../hooks/quick-reports';
import { SubmissionType } from '../../models/common';
import { mapIncidentCategory, mapQuickReportFollowUpStatus, mapQuickReportLocationType } from '../../utils/helpers';
import { ResponseExtraDataSection } from '../ReponseExtraDataSection/ResponseExtraDataSection';

export default function QuickReportDetails(): FunctionComponent {
  const { quickReportId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);

  const quickReportQuery = useSuspenseQuery(quickReportDetailsQueryOptions(currentElectionRoundId, quickReportId));
  const quickReport = quickReportQuery.data;
  const { invalidate } = useRouter();

  const updateQuickReportFollowUpStatusMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      followUpStatus,
    }: {
      electionRoundId: string;
      followUpStatus: QuickReportFollowUpStatus;
    }) => {
      return authApi.put<void>(`/election-rounds/${electionRoundId}/quick-reports/${quickReportId}:status`, {
        followUpStatus,
      });
    },

    onSuccess: (_data, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Follow-up status updated',
      });

      invalidate();
      void queryClient.invalidateQueries({ queryKey: quickReportKeys.all(electionRoundId) });
    },

    onError: () => {
      toast({
        title: 'Error updating follow up status',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  function handleFollowUpStatusChange(followUpStatus: QuickReportFollowUpStatus): void {
    updateQuickReportFollowUpStatusMutation.mutate({ electionRoundId: currentElectionRoundId, followUpStatus });
  }

  const prevSearch = usePrevSearch();

  return (
    <Layout
      backButton={<NavigateBack to='/responses' search={prevSearch} />}
      breadcrumbs={<></>}
      title={quickReport.id}>
      <div className='flex flex-col gap-4'>
        <Card>
          <CardContent className='flex flex-col gap-4 pt-6'>
            <div className='flex gap-2'>
              <p>Observer:</p>
              <Link
                className='flex gap-1 font-bold text-purple-500'
                to='/responses'
                search={{ searchText: quickReport.monitoringObserverId, tab: 'quick-reports', viewBy: 'byEntry' }}
                target='_blank'
                preload={false}>
                {quickReport.observerName}
                <ArrowTopRightOnSquareIcon className='w-4' />
              </Link>
            </div>

            <div>
              <p className='font-bold'>Time submitted</p>
              {quickReport.timestamp && <p>{format(quickReport.timestamp, DateTimeFormat)}</p>}
            </div>

            <div>
              <p className='font-bold'>Location type</p>
              <p>{mapQuickReportLocationType(quickReport.quickReportLocationType)}</p>
            </div>

            {quickReport.pollingStationDetails && (
              <div>
                <p className='font-bold'>Polling station details</p>
                <p>{quickReport.pollingStationDetails}</p>
              </div>
            )}

            {quickReport.level1 && (
              <div className='flex gap-4'>
                <div>
                  <p className='font-bold'>Location - L1</p>
                  {quickReport.level1}
                </div>
                {quickReport.level2 && (
                  <div>
                    <p className='font-bold'>Location - L2</p>
                    {quickReport.level2}
                  </div>
                )}
                {quickReport.level3 && (
                  <div>
                    <p className='font-bold'>Location - L3</p>
                    {quickReport.level3}
                  </div>
                )}
                {quickReport.level4 && (
                  <div>
                    <p className='font-bold'>Location - L4</p>
                    {quickReport.level4}
                  </div>
                )}
                {quickReport.level5 && (
                  <div>
                    <p className='font-bold'>Location - L5</p>
                    {quickReport.level5}
                  </div>
                )}
                <div>
                  <p className='font-bold'>Number</p>
                  <p>{quickReport.number}</p>
                </div>
              </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className='flex justify-between mb-4'>
              <div>Quick report</div>
              <Select
                onValueChange={handleFollowUpStatusChange}
                defaultValue={quickReport.followUpStatus}
                value={quickReport.followUpStatus}
                disabled={!quickReport.isOwnObserver || electionRound?.status === ElectionRoundStatus.Archived}>
                <SelectTrigger className='w-[180px]'>
                  <SelectValue placeholder='Follow-up status' />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectItem
                      key={QuickReportFollowUpStatus.NotApplicable}
                      value={QuickReportFollowUpStatus.NotApplicable}>
                      {mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.NotApplicable)}
                    </SelectItem>
                    <SelectItem
                      key={QuickReportFollowUpStatus.NeedsFollowUp}
                      value={QuickReportFollowUpStatus.NeedsFollowUp}>
                      {mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.NeedsFollowUp)}
                    </SelectItem>
                    <SelectItem key={QuickReportFollowUpStatus.Resolved} value={QuickReportFollowUpStatus.Resolved}>
                      {mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.Resolved)}
                    </SelectItem>
                  </SelectGroup>
                </SelectContent>
              </Select>
            </CardTitle>
            <Separator />
          </CardHeader>

          <CardContent className='text-[#374151] flex flex-col gap-6'>
            <div>
              <p className='font-bold'>Incident category</p>
              <p>{mapIncidentCategory(quickReport.incidentCategory)}</p>
            </div>

            <div>
              <p className='font-bold'>Issue title</p>
              <p>{quickReport.title}</p>
            </div>

            <div>
              <p className='font-bold'>Description</p>
              <p>{quickReport.description}</p>
            </div>

            {!!quickReport.attachments && quickReport.attachments.length > 0 && (
              <ResponseExtraDataSection
                attachments={quickReport.attachments.map((a) => ({
                  ...a,
                  timeSubmitted: quickReport.timestamp,
                }))}
                notes={[]}
                aggregateDisplay={false}
                submissionType={SubmissionType.QuickReport}
              />
            )}
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
}
