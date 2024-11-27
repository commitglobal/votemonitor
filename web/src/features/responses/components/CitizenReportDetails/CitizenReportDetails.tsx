import { authApi } from '@/common/auth-api';
import {
  CitizenReportFollowUpStatus,
  ElectionRoundStatus,
  FormSubmissionFollowUpStatus,
  type FunctionComponent,
} from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { queryClient } from '@/main';
import { citizenReportDetailsQueryOptions, Route } from '@/routes/responses/citizen-reports/$citizenReportId';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useRouter } from '@tanstack/react-router';
import { citizenReportKeys } from '../../hooks/citizen-reports';
import PreviewAnswer from '../PreviewAnswer/PreviewAnswer';
import { SubmissionType } from '../../models/common';
import { mapCitizenReportFollowUpStatus } from '../../utils/helpers';
import { usePrevSearch } from '@/common/prev-search-store';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { DateTimeFormat } from '@/common/formats';
import { format } from 'date-fns';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';

export default function CitizenReportDetails(): FunctionComponent {
  const { citizenReportId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);

  const { data: citizenReport } = useSuspenseQuery(
    citizenReportDetailsQueryOptions(currentElectionRoundId, citizenReportId)
  );

  const router = useRouter();
  const prevSearch = usePrevSearch();

  const updateSubmissionFollowUpStatusMutation = useMutation({
    mutationKey: citizenReportKeys.detail(currentElectionRoundId, citizenReportId),
    mutationFn: ({
      electionRoundId,
      followUpStatus,
    }: {
      electionRoundId: string;
      followUpStatus: FormSubmissionFollowUpStatus;
    }) => {
      return authApi.put<void>(`/election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}:status`, {
        followUpStatus,
      });
    },

    onSuccess: async (_, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Follow-up status updated',
      });

      await queryClient.invalidateQueries({ queryKey: citizenReportKeys.all(electionRoundId) });
      router.invalidate();
    },

    onError: () => {
      toast({
        title: 'Error updating follow up status',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  function handleFollowUpStatusChange(followUpStatus: FormSubmissionFollowUpStatus): void {
    updateSubmissionFollowUpStatusMutation.mutate({ electionRoundId: currentElectionRoundId, followUpStatus });
  }

  return (
    <Layout
      backButton={<NavigateBack to='/responses' search={prevSearch} />}
      breadcrumbs={<></>}
      title={`#${citizenReport.citizenReportId}`}>
      <div className='flex flex-col gap-4'>
        <Card>
          <CardContent className='flex flex-col gap-4 pt-6'>
            <div>
              <p className='font-bold'>Time submitted</p>
              {citizenReport.timeSubmitted && <p>{format(citizenReport.timeSubmitted, DateTimeFormat)}</p>}
            </div>

            {citizenReport.level1 && (
              <div className='flex gap-4'>
                <div>
                  <p className='font-bold'>Location - L1</p>
                  {citizenReport.level1}
                </div>
                {citizenReport.level2 && (
                  <div>
                    <p className='font-bold'>Location - L2</p>
                    {citizenReport.level2}
                  </div>
                )}
                {citizenReport.level3 && (
                  <div>
                    <p className='font-bold'>Location - L3</p>
                    {citizenReport.level3}
                  </div>
                )}
                {citizenReport.level4 && (
                  <div>
                    <p className='font-bold'>Location - L4</p>
                    {citizenReport.level4}
                  </div>
                )}
                {citizenReport.level5 && (
                  <div>
                    <p className='font-bold'>Location - L5</p>
                    {citizenReport.level5}
                  </div>
                )}
              </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className='flex justify-between mb-4'>
              <div>
                {citizenReport.formCode}: {citizenReport.formName[citizenReport.formDefaultLanguage]}
              </div>
              <Select
                onValueChange={handleFollowUpStatusChange}
                defaultValue={citizenReport.followUpStatus}
                disabled={electionRound?.status === ElectionRoundStatus.Archived}
                value={citizenReport.followUpStatus}>
                <SelectTrigger className='w-[180px]'>
                  <SelectValue placeholder='Follow-up status' />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectItem
                      key={CitizenReportFollowUpStatus.NotApplicable}
                      value={CitizenReportFollowUpStatus.NotApplicable}>
                      {mapCitizenReportFollowUpStatus(CitizenReportFollowUpStatus.NotApplicable)}
                    </SelectItem>
                    <SelectItem
                      key={CitizenReportFollowUpStatus.NeedsFollowUp}
                      value={CitizenReportFollowUpStatus.NeedsFollowUp}>
                      {mapCitizenReportFollowUpStatus(CitizenReportFollowUpStatus.NeedsFollowUp)}
                    </SelectItem>
                    <SelectItem key={CitizenReportFollowUpStatus.Resolved} value={CitizenReportFollowUpStatus.Resolved}>
                      {mapCitizenReportFollowUpStatus(CitizenReportFollowUpStatus.Resolved)}
                    </SelectItem>
                  </SelectGroup>
                </SelectContent>
              </Select>
            </CardTitle>
            <Separator />
          </CardHeader>

          <CardContent className='flex flex-col gap-10'>
            {citizenReport.questions.map((question, index) => {
              const answer = citizenReport.answers.find(({ questionId }) => questionId === question.id);
              const notes = citizenReport.notes.filter(({ questionId }) => questionId === question.id);
              const attachments = citizenReport.attachments.filter(({ questionId }) => questionId === question.id);

              return (
                <PreviewAnswer
                  key={index}
                  submissionType={SubmissionType.CitizenReport}
                  question={question}
                  answer={answer}
                  notes={notes}
                  attachments={attachments}
                  defaultLanguage={citizenReport.formDefaultLanguage}
                />
              );
            })}
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
}
