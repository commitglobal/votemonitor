import { DateTimeFormat } from '@/common/formats';
import { usePrevSearch } from '@/common/prev-search-store';
import { ElectionRoundStatus, FormSubmissionFollowUpStatus, FormType, FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { LanguageBadge } from '@/components/ui/language-badge';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { queryClient } from '@/main';
import { Route, formSubmissionDetailsQueryOptions } from '@/routes/(app)/responses/form-submissions/$submissionId';
import API from '@/services/api';
import { ArrowTopRightOnSquareIcon } from '@heroicons/react/24/outline';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { Link, useRouter } from '@tanstack/react-router';
import { format } from 'date-fns';
import { useState } from 'react';
import { toast } from 'sonner';
import { formSubmissionsByEntryKeys, formSubmissionsByObserverKeys } from '../../hooks/form-submissions-queries';
import { SubmissionType } from '../../models/common';
import { mapFormSubmissionFollowUpStatus } from '../../utils/helpers';
import PreviewAnswer from '../PreviewAnswer/PreviewAnswer';

export default function FormSubmissionDetails(): FunctionComponent {
  const { submissionId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);

  const { data: formSubmission } = useSuspenseQuery(
    formSubmissionDetailsQueryOptions(currentElectionRoundId, submissionId)
  );
  const prevSearch = usePrevSearch();

  const router = useRouter();

  const updateSubmissionFollowUpStatusMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      followUpStatus,
    }: {
      electionRoundId: string;
      followUpStatus: FormSubmissionFollowUpStatus;
    }) => {
      return API.put<void>(`/election-rounds/${electionRoundId}/form-submissions/${submissionId}:status`, {
        followUpStatus,
      });
    },

    onSuccess: async (_, { electionRoundId }) => {
      toast.success('Follow-up status updated');

      await queryClient.invalidateQueries({ queryKey: formSubmissionsByEntryKeys.all(electionRoundId) });
      await queryClient.invalidateQueries({ queryKey: formSubmissionsByObserverKeys.all(electionRoundId) });
      router.invalidate();
    },

    onError: () => {
      toast.error('Error updating follow up status', {
        description: 'Please contact tech support',
      });
    },
  });

  const [selectedLanguage, setSelectedLanguage] = useState<string>(formSubmission.defaultLanguage);

  function handleFollowUpStatusChange(followUpStatus: FormSubmissionFollowUpStatus): void {
    updateSubmissionFollowUpStatusMutation.mutate({ electionRoundId: currentElectionRoundId, followUpStatus });
  }

  return (
    <Layout
      backButton={<NavigateBack to='/responses' search={prevSearch} />}
      breadcrumbs={<></>}
      title={`#${formSubmission.submissionId}`}>
      <div className='flex flex-col gap-4'>
        <Card>
          <CardContent className='flex flex-row justify-between gap-4'>
            <section className='flex flex-col gap-4 pt-6'>
              <div className='flex gap-2'>
                <p>Observer:</p>
                <Link
                  className='flex gap-1 font-bold text-purple-500'
                  to='/responses'
                  search={{ searchText: formSubmission.monitoringObserverId, tab: 'form-answers', viewBy: 'byEntry' }}
                  target='_blank'
                  preload={false}>
                  {formSubmission.observerName}
                  <ArrowTopRightOnSquareIcon className='w-4' />
                </Link>
              </div>

              <div>
                <p className='font-bold'>Time submitted</p>
                {formSubmission.timeSubmitted && <p>{format(formSubmission.timeSubmitted, DateTimeFormat)}</p>}
              </div>

              {formSubmission.level1 && (
                <div className='flex gap-4'>
                  <div>
                    <p className='font-bold'>Location - L1</p>
                    {formSubmission.level1}
                  </div>
                  {formSubmission.level2 && (
                    <div>
                      <p className='font-bold'>Location - L2</p>
                      {formSubmission.level2}
                    </div>
                  )}
                  {formSubmission.level3 && (
                    <div>
                      <p className='font-bold'>Location - L3</p>
                      {formSubmission.level3}
                    </div>
                  )}
                  {formSubmission.level4 && (
                    <div>
                      <p className='font-bold'>Location - L4</p>
                      {formSubmission.level4}
                    </div>
                  )}
                  {formSubmission.level5 && (
                    <div>
                      <p className='font-bold'>Location - L5</p>
                      {formSubmission.level5}
                    </div>
                  )}
                  <div>
                    <p className='font-bold'>Number</p>
                    <p>{formSubmission.number}</p>
                  </div>
                </div>
              )}
            </section>
            <section className='flex flex-col pt-10'>
              {formSubmission.languages.length > 1 ? (
                <div>
                  <div className='mb-2'>
                    <p className='font-bold'>Preffered language</p>
                  </div>
                  <Select onValueChange={setSelectedLanguage} defaultValue={selectedLanguage} value={selectedLanguage}>
                    <SelectTrigger className='w-[180px]'>
                      <SelectValue placeholder='Language' />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectGroup>
                        {formSubmission.languages.map((language) => (
                          <SelectItem key={language} value={language}>
                            {LanguageBadge({ languageCode: language, variant: 'unstyled', displayMode: 'english' })}
                          </SelectItem>
                        ))}
                      </SelectGroup>
                    </SelectContent>
                  </Select>
                </div>
              ) : null}
            </section>
          </CardContent>
        </Card>
        {formSubmission.formType === FormType.PSI ? (
          <Card>
            <CardHeader>
              <div className='flex gap-4'>
                <div>
                  <p className='font-bold'>Arrival time</p>
                  {<p>{formSubmission.arrivalTime ? format(formSubmission.arrivalTime, DateTimeFormat) : '-'}</p>}
                </div>

                <div>
                  <p className='font-bold'>Departure time</p>
                  {<p>{formSubmission.departureTime ? format(formSubmission.departureTime, DateTimeFormat) : '-'}</p>}
                </div>
              </div>
              {formSubmission.breaks ? (
                <div>
                  {formSubmission.breaks.length > 0 ? (
                    <ul className='space-y-4'>
                      {formSubmission.breaks.map((breakItem, index) => (
                        <li key={index}>
                          <div>
                            <p className='font-bold'>Break #{index + 1}</p>
                            <p>Start: {format(breakItem.start, DateTimeFormat)}</p>
                            {breakItem.end ? <p>End: {format(breakItem.end, DateTimeFormat)}</p> : <p>End: -</p>}
                          </div>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p>No breaks</p>
                  )}
                </div>
              ) : null}
            </CardHeader>
          </Card>
        ) : null}

        <Card>
          <CardHeader>
            <CardTitle className='flex justify-between mb-4'>
              <div>
                {formSubmission.formCode}: {formSubmission.formType}
              </div>
              <Select
                onValueChange={handleFollowUpStatusChange}
                defaultValue={formSubmission.followUpStatus}
                value={formSubmission.followUpStatus}
                disabled={!formSubmission.isOwnObserver || electionRound?.status === ElectionRoundStatus.Archived}>
                <SelectTrigger className='w-[180px]'>
                  <SelectValue placeholder='Follow-up status' />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    <SelectItem
                      key={FormSubmissionFollowUpStatus.NotApplicable}
                      value={FormSubmissionFollowUpStatus.NotApplicable}>
                      {mapFormSubmissionFollowUpStatus(FormSubmissionFollowUpStatus.NotApplicable)}
                    </SelectItem>
                    <SelectItem
                      key={FormSubmissionFollowUpStatus.NeedsFollowUp}
                      value={FormSubmissionFollowUpStatus.NeedsFollowUp}>
                      {mapFormSubmissionFollowUpStatus(FormSubmissionFollowUpStatus.NeedsFollowUp)}
                    </SelectItem>
                    <SelectItem
                      key={FormSubmissionFollowUpStatus.Resolved}
                      value={FormSubmissionFollowUpStatus.Resolved}>
                      {mapFormSubmissionFollowUpStatus(FormSubmissionFollowUpStatus.Resolved)}
                    </SelectItem>
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

              return (
                <PreviewAnswer
                  key={index}
                  submissionType={SubmissionType.FormSubmission}
                  question={question}
                  answer={answer}
                  notes={notes}
                  attachments={attachments}
                  defaultLanguage={selectedLanguage}
                />
              );
            })}
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
}
