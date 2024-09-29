import { authApi } from '@/common/auth-api';
import { IssueReportFollowUpStatus, type FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { queryClient } from '@/main';
import { issueReportDetailsQueryOptions, Route } from '@/routes/responses/issue-reports/$issueReportId';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useRouter } from '@tanstack/react-router';
import { issueReportsByEntryKeys, issueReportsByObserverKeys } from '../../hooks/issue-reports-queries';
import { SubmissionType } from '../../models/common';
import PreviewAnswer from '../PreviewAnswer/PreviewAnswer';
import { mapIssueReportFollowUpStatus } from '../../utils/helpers';

export default function IssueReportDetails(): FunctionComponent {
  const { issueReportId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: issueReport } = useSuspenseQuery(
    issueReportDetailsQueryOptions(currentElectionRoundId, issueReportId)
  );

  const router = useRouter();

  const updateSubmissionFollowUpStatusMutation = useMutation({
    mutationKey: issueReportsByEntryKeys.detail(currentElectionRoundId, issueReportId),
    mutationFn: ({
      electionRoundId,
      followUpStatus,
    }: {
      electionRoundId: string;
      followUpStatus: IssueReportFollowUpStatus;
    }) => {
      return authApi.put<void>(`/election-rounds/${electionRoundId}/issue-reports/${issueReportId}:status`, {
        followUpStatus,
      });
    },

    onSuccess: async (_, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Follow-up status updated',
      });

      await queryClient.invalidateQueries({ queryKey: issueReportsByEntryKeys.all(electionRoundId) });
      await queryClient.invalidateQueries({ queryKey: issueReportsByObserverKeys.all(electionRoundId) });

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

  function handleFollowUpStatusChange(followUpStatus: IssueReportFollowUpStatus): void {
    updateSubmissionFollowUpStatusMutation.mutate({ electionRoundId: currentElectionRoundId, followUpStatus });
  }

  return (
    <Layout title={`#${issueReport.issueReportId}`}>
      <div className='flex flex-col gap-4'>
        <Card>
          <CardHeader>
            <CardTitle className='flex justify-between mb-4'>
              <div>
                {issueReport.formCode}: {issueReport.formName[issueReport.formDefaultLanguage]}
              </div>
              <Select
                onValueChange={handleFollowUpStatusChange}
                defaultValue={issueReport.followUpStatus}
                value={issueReport.followUpStatus}>
                <SelectTrigger className='w-[180px]'>
                  <SelectValue placeholder='Follow-up status' />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectItem
                      key={IssueReportFollowUpStatus.NotApplicable}
                      value={IssueReportFollowUpStatus.NotApplicable}>
                      {mapIssueReportFollowUpStatus(IssueReportFollowUpStatus.NotApplicable)}
                    </SelectItem>
                    <SelectItem
                      key={IssueReportFollowUpStatus.NeedsFollowUp}
                      value={IssueReportFollowUpStatus.NeedsFollowUp}>
                      {mapIssueReportFollowUpStatus(IssueReportFollowUpStatus.NeedsFollowUp)}
                    </SelectItem>
                    <SelectItem key={IssueReportFollowUpStatus.Resolved} value={IssueReportFollowUpStatus.Resolved}>
                      {mapIssueReportFollowUpStatus(IssueReportFollowUpStatus.Resolved)}
                    </SelectItem>
                  </SelectGroup>
                </SelectContent>
              </Select>
            </CardTitle>
            <Separator />
          </CardHeader>

          <CardContent className='flex flex-col gap-10'>
            {issueReport.questions.map((question, index) => {
              const answer = issueReport.answers.find(({ questionId }) => questionId === question.id);
              const notes = issueReport.notes.filter(({ questionId }) => questionId === question.id);
              const attachments = issueReport.attachments.filter(({ questionId }) => questionId === question.id);

              return (
                <PreviewAnswer
                  key={index}
                  submissionType={SubmissionType.IssueReport}
                  question={question}
                  answer={answer}
                  notes={notes}
                  attachments={attachments}
                  defaultLanguage={issueReport.formDefaultLanguage}
                />
              );
            })}
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
}
