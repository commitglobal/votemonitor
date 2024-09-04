import { authApi } from '@/common/auth-api';
import { FollowUpStatus, FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { queryClient } from '@/main';
import { formSubmissionDetailsQueryOptions, Route } from '@/routes/responses/$submissionId';
import { ArrowTopRightOnSquareIcon } from '@heroicons/react/24/outline';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { Link, useRouter } from '@tanstack/react-router';
import { formSubmissionsByEntryKeys, formSubmissionsByObserverKeys } from '../../hooks/form-submissions-queries';
import PreviewAnswer from '../PreviewAnswer/PreviewAnswer';

export default function FormSubmissionDetails(): FunctionComponent {
  const { submissionId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const { data: formSubmission } = useSuspenseQuery(formSubmissionDetailsQueryOptions(currentElectionRoundId, submissionId));

  const router = useRouter();

  const updateSubmissionFollowUpStatusMutation = useMutation({
    mutationFn: ({ electionRoundId, followUpStatus }: { electionRoundId: string; followUpStatus: FollowUpStatus }) => {
      return authApi.put<void>(
        `/election-rounds/${electionRoundId}/form-submissions/${submissionId}:status`,
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

      await queryClient.invalidateQueries({ queryKey: formSubmissionsByEntryKeys.all });
      await queryClient.invalidateQueries({ queryKey: formSubmissionsByObserverKeys.all });
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
    <Layout title={`#${formSubmission.submissionId}`}>
      <div className='flex flex-col gap-4'>
        <Card className='max-w-4xl'>
          <CardContent className='pt-6 flex flex-col gap-4'>
            <div className='flex gap-2'>
              <p>Observer:</p>
              <Link
                search
                className='text-purple-500 font-bold flex gap-1'
                to='/monitoring-observers/view/$monitoringObserverId/$tab'
                params={{ monitoringObserverId: formSubmission.monitoringObserverId, tab: 'details' }}
                target='_blank'
                preload={false}>
                {formSubmission.observerName}
                <ArrowTopRightOnSquareIcon className='w-4' />
              </Link>
            </div>

            <div className='flex gap-2'>
              <p>Phone number:</p>
              {formSubmission.phoneNumber}
            </div>

            <div className='flex gap-4'>
              <div className='flex gap-2'>
                <p>Location - L1:</p>
                {formSubmission.level1}
              </div>
              {formSubmission.level2 && <div className='flex gap-2'>
                <p>Location - L2:</p>
                {formSubmission.level2}
              </div>}
              {formSubmission.level3 && <div className='flex gap-2'>
                <p>Location - L3:</p>
                {formSubmission.level3}
              </div>}
              {formSubmission.level4 && <div className='flex gap-2'>
                <p>Location - L4:</p>
                {formSubmission.level4}
              </div>}
              {formSubmission.level5 && <div className='flex gap-2'>
                <p>Location - L5:</p>
                {formSubmission.level5}
              </div>}
              <p>Number:</p>
              #{formSubmission.number}
            </div>
          </CardContent>
        </Card>

        <Card className='max-w-4xl'>
          <CardHeader>
            <CardTitle className='mb-4 flex justify-between'>
              <div>
                {formSubmission.formCode}: {formSubmission.formType}
              </div>
              <Select onValueChange={handleFollowUpStatusChange} defaultValue={formSubmission.followUpStatus} value={formSubmission.followUpStatus}>
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
            {formSubmission.questions.map((question, index) => {
              const answer = formSubmission.answers.find(({ questionId }) => questionId === question.id);
              const notes = formSubmission.notes.filter(({ questionId }) => questionId === question.id);
              const attachments = formSubmission.attachments.filter(({ questionId }) => questionId === question.id);

              return <PreviewAnswer
                key={index}
                question={question}
                answer={answer}
                notes={notes}
                attachments={attachments}
                defaultLanguage={formSubmission.defaultLanguage} />
            })}
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
}
