import { authApi } from '@/common/auth-api';
import { FollowUpStatus, type FunctionComponent } from '@/common/types';
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

export default function CitizenReportDetails(): FunctionComponent {
  const { citizenReportId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const { data: citizenReport } = useSuspenseQuery(citizenReportDetailsQueryOptions(currentElectionRoundId, citizenReportId));

  const router = useRouter();

  const updateSubmissionFollowUpStatusMutation = useMutation({
    mutationKey: citizenReportKeys.detail(currentElectionRoundId, citizenReportId),
    mutationFn: ({ electionRoundId, followUpStatus }: { electionRoundId: string; followUpStatus: FollowUpStatus }) => {
      return authApi.put<void>(
        `/election-rounds/${electionRoundId}/citizen-reports/${citizenReportId}:status`,
        {
          followUpStatus
        }
      );
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
        variant: 'destructive'
      });
    }
  });

  function handleFollowUpStatusChange(followUpStatus: FollowUpStatus): void {
    updateSubmissionFollowUpStatusMutation.mutate({ electionRoundId: currentElectionRoundId, followUpStatus });
  }

  return (
    <Layout title={`#${citizenReport.citizenReportId}`}>
      <div className='flex flex-col gap-4'>
        <Card>
          <CardHeader>
            <CardTitle className='flex justify-between mb-4'>
              <div>
                {citizenReport.formCode}: {citizenReport.formName[citizenReport.formDefaultLanguage]}
              </div>
              <Select onValueChange={handleFollowUpStatusChange} defaultValue={citizenReport.followUpStatus} value={citizenReport.followUpStatus}>
                <SelectTrigger className="w-[180px]">
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

          <CardContent className='flex flex-col gap-10'>
            {citizenReport.questions.map((question, index) => {
              const answer = citizenReport.answers.find(({ questionId }) => questionId === question.id);
              const notes = citizenReport.notes.filter(({ questionId }) => questionId === question.id);
              const attachments = citizenReport.attachments.filter(({ questionId }) => questionId === question.id);

              return <PreviewAnswer
              key={index}
              question={question}
              answer={answer}
              notes={notes}
              attachments={attachments}
              defaultLanguage={citizenReport.formDefaultLanguage} />
            })}
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
}
