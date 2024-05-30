import { authApi } from '@/common/auth-api';
import { DateTimeFormat } from '@/common/formats';
import { usePrevSearch } from '@/common/prev-search-store';
import { FollowUpStatus, type FunctionComponent } from '@/common/types';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { Route, quickReportDetailsQueryOptions } from '@/routes/responses/quick-reports/$quickReportId';
import { ArrowTopRightOnSquareIcon } from '@heroicons/react/24/outline';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { Link, useRouter } from '@tanstack/react-router';
import { format } from 'date-fns';
import { quickReportKeys } from '../../hooks/quick-reports';
import { mapQuickReportLocationType } from '../../utils/helpers';

export default function QuickReportDetails(): FunctionComponent {
  const { quickReportId } = Route.useParams();
  const quickReportQuery = useSuspenseQuery(quickReportDetailsQueryOptions(quickReportId));
  const quickReport = quickReportQuery.data;
  const { invalidate } = useRouter();

  const updateQuickReportFollowUpStatusMutation = useMutation({
    mutationKey: quickReportKeys.all,
    mutationFn: (followUpStatus: FollowUpStatus) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      return authApi.put<void>(`/election-rounds/${electionRoundId}/quick-reports/${quickReportId}:status`, {
        followUpStatus,
      });
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Follow-up status updated',
      });

      invalidate();
      void queryClient.invalidateQueries({ queryKey: quickReportKeys.all });
    },

    onError: () => {
      toast({
        title: 'Error updating follow up status',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  function handleFolowUpStatusChange(followUpStatus: FollowUpStatus): void {
    updateQuickReportFollowUpStatusMutation.mutate(followUpStatus);
  }

  const prevSearch = usePrevSearch();

  return (
    <Layout
      backButton={<NavigateBack to='/responses' search={prevSearch} />}
      breadcrumbs={
        <div className='breadcrumbs flex flex-row gap-2 mb-4'>
          <Link className='crumb' to='/responses' preload='intent' search={prevSearch as any}>
            responses
          </Link>
          <Link className='crumb'>{quickReport.id}</Link>
        </div>
      }
      title={quickReport.id}>
      <Card className='max-w-4xl'>
        <CardHeader>
          <CardTitle className='mb-4 flex justify-between'>
            <div>Quick report</div>
            <Select
              onValueChange={handleFolowUpStatusChange}
              defaultValue={quickReport.followUpStatus}
              value={quickReport.followUpStatus}>
              <SelectTrigger className='w-[180px]'>
                <SelectValue placeholder='Follow-up status' />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  <SelectItem value={FollowUpStatus.NotApplicable}>Not Applicable</SelectItem>
                  <SelectItem value={FollowUpStatus.NeedsFollowUp}>Needs Follow-Up</SelectItem>
                  <SelectItem value={FollowUpStatus.Resolved}>Resolved</SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>
          </CardTitle>
          <Separator />
        </CardHeader>

        <CardContent className='text-[#374151] flex flex-col gap-6'>
          <div>
            <p className='font-bold'>Time submitted</p>
            {quickReport.timestamp && <p>{format(quickReport.timestamp, DateTimeFormat)}</p>}
          </div>

          <div>
            <p className='font-bold'>Issue title</p>
            <p>{quickReport.title}</p>
          </div>

          <div>
            <p className='font-bold'>Description</p>
            <p>{quickReport.description}</p>
          </div>

          {quickReport.attachments.length > 0 && (
            <div>
              <p className='font-bold pb-2'>Media files</p>
              <div className='flex flex-wrap gap-4'>
                {quickReport.attachments.map((attachment) => (
                  <Dialog>
                    <DialogTrigger>
                      <button type='button'>
                        <img alt={attachment.fileName} className='rounded-lg h-44 w-44' src={attachment.presignedUrl} />
                      </button>
                    </DialogTrigger>
                    <DialogContent className='max-w-5xl'>
                      <div className='flex justify-center'>
                        <img alt={attachment.fileName} src={attachment.presignedUrl} />
                      </div>
                    </DialogContent>
                  </Dialog>
                ))}
              </div>
            </div>
          )}

          <div>
            <p className='font-bold'>Observer</p>
            <Link
              search
              className='text-purple-500 flex gap-1'
              to='/monitoring-observers/view/$monitoringObserverId/$tab'
              params={{ monitoringObserverId: quickReport.monitoringObserverId, tab: 'details' }}
              target='_blank'
              preload={false}>
              {quickReport.firstName} {quickReport.lastName}
              <ArrowTopRightOnSquareIcon className='w-4' />
            </Link>
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
                <p className='font-bold'>Station</p>
                <p>{quickReport.number}</p>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </Layout>
  );
}
