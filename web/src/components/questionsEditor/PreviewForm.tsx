import { BaseAnswer, BaseAnswerSchema, BaseQuestion } from '@/common/types';
import { useEffect, useState } from 'react';
import PreviewQuestionFactory from './preview/PreviewQuestionFactory';
import { Progress } from '../ui/progress';
import { z } from 'zod';
import { Button } from '../ui/button';
import { Card, CardContent, CardHeader } from '../ui/card';
import { ChevronLeftIcon, ChevronRightIcon } from '@heroicons/react/24/outline';
export interface PreviewFormProps {
  languageCode: string;
  localQuestions: BaseQuestion[];
  activeQuestionId: string | undefined;
  setActiveQuestionId: (questionId: string) => void;
}

export const ResponseDataSchema = z.record(BaseAnswerSchema);
export type ResponseData = z.infer<typeof ResponseDataSchema>;

function PreviewForm({ languageCode, localQuestions, activeQuestionId, setActiveQuestionId }: PreviewFormProps) {
  const [currentQuestion, setCurrentQuestion] = useState<BaseQuestion | undefined>();
  const [responseData, setResponseData] = useState<ResponseData>({});
  const [progress, setProgress] = useState(0);
  useEffect(() => {
    if (activeQuestionId === localQuestions[localQuestions.length - 1]?.id) {
      setProgress(100);
      return;
    }

    const currentQuestionIndex = localQuestions.findIndex((q) => q.id === activeQuestionId);
    const percentage = (currentQuestionIndex / localQuestions.length) * 100;

    setProgress(percentage);
  }, [activeQuestionId, localQuestions]);

  useEffect(() => {
    const currentQuestionIndex = localQuestions.findIndex((q) => q.id === activeQuestionId);
    const currentQuestion = localQuestions[currentQuestionIndex];
    setCurrentQuestion(currentQuestion);
  }, [activeQuestionId, localQuestions]);

  function resetProgress() {
    setActiveQuestionId(localQuestions[0]?.id!);
  }

  function onSubmitAnswer(answer: BaseAnswer) {
    setResponseData({
      ...responseData,
      questionId: answer,
    });

    const nextQuestionId = getNextQuestionId(answer.questionId);
    setActiveQuestionId(nextQuestionId);
  }

  function onBackButtonClicked() {
    var previousQuestionId = getPreviousQuestionId(activeQuestionId);
    setActiveQuestionId(previousQuestionId);
  }

  function onNextButtonClicked() {
    var previousQuestionId = getNextQuestionId(activeQuestionId);
    setActiveQuestionId(previousQuestionId);
  }

  function getNextQuestionId(questionId?: string): string {
    const currentQuestionIndex = localQuestions.findIndex((q) => q.id === questionId);

    if (currentQuestionIndex === -1) throw new Error('Question not found');

    return localQuestions[currentQuestionIndex + 1]?.id || 'end';
  }

  function getPreviousQuestionId(questionId?: string): string {
    const currentQuestionIndex = localQuestions.findIndex((q) => q.id === questionId);

    if (currentQuestionIndex === -1) throw new Error('Question not found');

    return localQuestions[currentQuestionIndex - 1]?.id || localQuestions[0]?.id || '';
  }

  return (
    <Card className='w-full'>
      <CardHeader className='p-4 bg-neutral-900 rounded-t-md'>
        <h3 className='text-white'>Question preview</h3>
      </CardHeader>
      <CardContent className='p-12'>
        {currentQuestion ? (
          <div className='flex h-full w-full flex-col justify-between px-6 pb-3 pt-6 '>
            {/* <Button className='mb-4' type='button' onClick={resetProgress}>
            Start from the beginning
          </Button> */}
            <div>
              {!!currentQuestion ? (
                <PreviewQuestionFactory
                  languageCode={languageCode}
                  question={currentQuestion}
                  answer={responseData[activeQuestionId!]!}
                  isFirstQuestion={currentQuestion.id === localQuestions[0]?.id}
                  isLastQuestion={currentQuestion.id === localQuestions[localQuestions.length - 1]?.id}
                  onSubmitAnswer={onSubmitAnswer}
                  onBackButtonClicked={onBackButtonClicked}
                />
              ) : activeQuestionId === 'end' ? (
                <div>Done!</div>
              ) : localQuestions.length ? (
                <div></div>
              ) : (
                <div>No questions available.</div>
              )}
            </div>
            <div className='nav-buttons flex gap-4 mt-4 justify-end'>
              <Button
                variant='outline'
                className='mb-4 flex justify-center items-center gap-2'
                type='button'
                onClick={onBackButtonClicked}>
                <ChevronLeftIcon width={18} />
                Back
              </Button>
              <Button
                className='mb-4 flex justify-center items-center gap-2'
                type='button'
                onClick={onNextButtonClicked}>
                Next
                <ChevronRightIcon width={18} />
              </Button>
            </div>
            {!!localQuestions.length && (
              <div className='mt-8'>
                <Progress value={progress} max={100} />
              </div>
            )}
          </div>
        ) : (
          <h2 className='text-2xl text-center'>Please select a question</h2>
        )}
      </CardContent>
    </Card>
  );
}
export default PreviewForm;
