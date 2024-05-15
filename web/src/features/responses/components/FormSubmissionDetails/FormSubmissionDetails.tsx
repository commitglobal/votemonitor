import { ArrowTopRightOnSquareIcon } from '@heroicons/react/24/outline';
import { FlagIcon } from '@heroicons/react/24/solid';
import { Link, useLoaderData } from '@tanstack/react-router';
import { format } from 'date-fns';
import { Fragment } from 'react';
import {
  isDateAnswer,
  isMultiSelectAnswer,
  isMultiSelectQuestion,
  isNumberAnswer,
  isRatingAnswer,
  isRatingQuestion,
  isSingleSelectAnswer,
  isSingleSelectQuestion,
  isTextAnswer,
} from '@/common/guards';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { FormItem } from '@/components/ui/form';
import { Label } from '@/components/ui/label';
import { Radio, RadioGroup } from '@/components/ui/radio-group';
import { RatingGroup } from '@/components/ui/ratings';
import { Separator } from '@/components/ui/separator';
import { Switch } from '@/components/ui/switch';
import type { FunctionComponent } from '@/common/types';
import { cn, ratingScaleToNumber } from '@/lib/utils';
import { QuestionExtraDataSection } from '../QuestionExtraDataSection/QuestionExtraDataSection';

export default function FormSubmissionDetails(): FunctionComponent {
  const formSubmission = useLoaderData({ from: '/responses/$submissionId' });

  return (
    <Layout title={`#${formSubmission.submissionId}`}>
      <div className='flex flex-col gap-4'>
        <Card className='max-w-4xl'>
          <CardContent className='pt-6 flex flex-col gap-4'>
            <div className='flex gap-2'>
              <p>Observer:</p>
              <Link
                className='text-purple-500 font-bold flex gap-1'
                to='/monitoring-observers/$monitoringObserverId'
                params={{ monitoringObserverId: formSubmission.monitoringObserverId }}
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

            <div className='flex gap-2'>
              <p>Station:</p>
              <Link
                className='text-purple-500 font-bold'
                to='/responses'
                search={{ pollingStationNumberFilter: formSubmission.number }}>
                #{formSubmission.number}
              </Link>
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
            </div>
          </CardContent>
        </Card>

        <Card className='max-w-4xl'>
          <CardHeader>
            <CardTitle className='mb-4 flex justify-between'>
              <div>
                {formSubmission.formCode}: {formSubmission.formType}
              </div>
              <Switch id='needs-followup'>Needs follow-up</Switch>
            </CardTitle>
            <Separator />
          </CardHeader>

          <CardContent className='flex flex-col gap-10'>
            {formSubmission.questions.map((question, index) => {
              const answer = formSubmission.answers.find(({ questionId }) => questionId === question.id);
              const notes = formSubmission.notes.filter(({ questionId }) => questionId === question.id);
              const attachments = formSubmission.attachments.filter(({ questionId }) => questionId === question.id);

              return (
                <div key={question.id} className='flex flex-col gap-4'>
                  <p className='text-gray-700 font-medium'>
                    {index + 1}: {question.text[formSubmission.defaultLanguage]}
                  </p>

                  {isSingleSelectQuestion(question) && (
                    <RadioGroup
                      defaultChecked
                      defaultValue={answer && isSingleSelectAnswer(answer) ? answer.selection?.optionId : ''}
                      className='flex gap-8 items-center'>
                      {question.options.map((option) => (
                        <Fragment key={option.id}>
                          <FormItem className='flex items-center gap-2 !mt-0'>
                            <Radio disabled value={option.id} />
                            <Label className='font-normal'>
                              {option.text[formSubmission.defaultLanguage]}
                              {option.isFlagged && <> (Flagged)</>}
                            </Label>
                            {option.isFlagged && <FlagIcon className={cn('text-destructive', 'w-4')} />}
                          </FormItem>
                          {option.isFreeText &&
                            answer &&
                            isSingleSelectAnswer(answer) &&
                            answer.selection?.optionId === option.id && <p>{answer.selection?.text ?? '-'}</p>}
                        </Fragment>
                      ))}
                    </RadioGroup>
                  )}

                  {isMultiSelectQuestion(question) &&
                    question.options.map((option) => {
                      const isOptionChecked =
                        answer &&
                        isMultiSelectAnswer(answer) &&
                        !!answer.selection?.find((selection) => selection.optionId === option.id);

                      return (
                        <FormItem key={option.id} className='flex flex-row items-start space-x-3 space-y-0'>
                          <Checkbox checked={isOptionChecked} disabled />
                          <Label>{option.text[formSubmission.defaultLanguage]}</Label>
                        </FormItem>
                      );
                    })}

                  {isRatingQuestion(question) && (
                    <RatingGroup
                      className='max-w-fit'
                      scale={ratingScaleToNumber(question.scale)}
                      defaultValue={answer && isRatingAnswer(answer) ? answer.value?.toString() : undefined}
                      disabled
                    />
                  )}

                  {answer ? (
                    <>
                      {isDateAnswer(answer) && <p>{answer.date ? format(answer.date, 'PPpp') : '-'}</p>}

                      {isNumberAnswer(answer) && <p>{answer.value ?? '-'}</p>}

                      {isTextAnswer(answer) && <p>{answer.text ?? '-'}</p>}
                    </>
                  ) : (
                    '-'
                  )}
                  {(attachments.length > 0 || notes.length > 0) && (
                    <QuestionExtraDataSection attachments={attachments} notes={notes} />
                  )}
                </div>
              );
            })}
          </CardContent>
        </Card>
      </div>
    </Layout>
  );
}
