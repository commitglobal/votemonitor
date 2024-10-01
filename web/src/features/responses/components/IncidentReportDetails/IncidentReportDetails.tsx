import { authApi } from '@/common/auth-api';
import { IncidentReportFollowUpStatus, type FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { queryClient } from '@/main';
import { incidentReportDetailsQueryOptions, Route } from '@/routes/responses/incident-reports/$incidentReportId';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useRouter } from '@tanstack/react-router';
import { incidentReportsByEntryKeys, incidentReportsByObserverKeys } from '../../hooks/incident-reports-queries';
import { SubmissionType } from '../../models/common';
import PreviewAnswer from '../PreviewAnswer/PreviewAnswer';
import { mapIncidentReportFollowUpStatus } from '../../utils/helpers';

export default function IncidentReportDetails(): FunctionComponent {
  const { incidentReportId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: incidentReport } = useSuspenseQuery(
    incidentReportDetailsQueryOptions(currentElectionRoundId, incidentReportId)
  );

  const router = useRouter();

  const updateSubmissionFollowUpStatusMutation = useMutation({
    mutationKey: incidentReportsByEntryKeys.detail(currentElectionRoundId, incidentReportId),
    mutationFn: ({
      electionRoundId,
      followUpStatus,
    }: {
      electionRoundId: string;
      followUpStatus: IncidentReportFollowUpStatus;
    }) => {
      return authApi.put<void>(`/election-rounds/${electionRoundId}/incident-reports/${incidentReportId}:status`, {
        followUpStatus,
      });
    },

    onSuccess: async (_, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Follow-up status updated',
      });

      await queryClient.invalidateQueries({ queryKey: incidentReportsByEntryKeys.all(electionRoundId) });
      await queryClient.invalidateQueries({ queryKey: incidentReportsByObserverKeys.all(electionRoundId) });

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

  function handleFollowUpStatusChange(followUpStatus: IncidentReportFollowUpStatus): void {
    updateSubmissionFollowUpStatusMutation.mutate({ electionRoundId: currentElectionRoundId, followUpStatus });
  }

  return (
    <Layout title={`#${incidentReport.incidentReportId}`}>
      <div className='flex flex-col gap-4'>
        <Card>
          <CardHeader>
            <CardTitle className='flex justify-between mb-4'>
              <div>
                {incidentReport.formCode}: {incidentReport.formName[incidentReport.formDefaultLanguage]}
              </div>
              <Select
                onValueChange={handleFollowUpStatusChange}
                defaultValue={incidentReport.followUpStatus}
                value={incidentReport.followUpStatus}>
                <SelectTrigger className='w-[180px]'>
                  <SelectValue placeholder='Follow-up status' />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectItem
                      key={IncidentReportFollowUpStatus.NotApplicable}
                      value={IncidentReportFollowUpStatus.NotApplicable}>
                      {mapIncidentReportFollowUpStatus(IncidentReportFollowUpStatus.NotApplicable)}
                    </SelectItem>
                    <SelectItem
                      key={IncidentReportFollowUpStatus.NeedsFollowUp}
                      value={IncidentReportFollowUpStatus.NeedsFollowUp}>
                      {mapIncidentReportFollowUpStatus(IncidentReportFollowUpStatus.NeedsFollowUp)}
                    </SelectItem>
                    <SelectItem key={IncidentReportFollowUpStatus.Resolved} value={IncidentReportFollowUpStatus.Resolved}>
                      {mapIncidentReportFollowUpStatus(IncidentReportFollowUpStatus.Resolved)}
                    </SelectItem>
                  </SelectGroup>
                </SelectContent>
              </Select>
            </CardTitle>
            <Separator />
          </CardHeader>

          <CardContent className='flex flex-col gap-10'>
            {incidentReport.questions.map((question, index) => {
              const answer = incidentReport.answers.find(({ questionId }) => questionId === question.id);
              const notes = incidentReport.notes.filter(({ questionId }) => questionId === question.id);
              const attachments = incidentReport.attachments.filter(({ questionId }) => questionId === question.id);

              return (
                <PreviewAnswer
                  key={index}
                  submissionType={SubmissionType.IncidentReport}
                  question={question}
                  answer={answer}
                  notes={notes}
                  attachments={attachments}
                  defaultLanguage={incidentReport.formDefaultLanguage}
                />
              );
            })}
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
}
