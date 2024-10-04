import { BaseAnswer, DisplayLogicCondition, QuestionType } from '@/common/types';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { ChevronLeftIcon, ChevronRightIcon } from '@heroicons/react/24/outline';
import { useFormAnswersStore } from '../answers-store';
import PreviewDateQuestion from './PreviewDateQuestion';
import PreviewMultiSelectQuestion from './PreviewMultiSelectQuestion';
import PreviewNumberQuestion from './PreviewNumberQuestion';
import PreviewRatingQuestion from './PreviewRatingQuestion';
import PreviewSingleSelectQuestion from './PreviewSingleSelectQuestion';
import PreviewTextQuestion from './PreviewTextQuestion';

import { isMultiSelectAnswer, isNumberAnswer, isRatingAnswer, isSingleSelectAnswer } from '@/common/guards';
import { Progress } from '@/components/ui/progress';
import { EditFormType } from '@/features/forms/components/EditForm/EditForm';
import { isNilOrWhitespace } from '@/lib/utils';
import { useFormContext, useWatch } from 'react-hook-form';

export interface PreviewQuestionProps {
  activeQuestionId: string | undefined;
  setActiveQuestionId: (questionId: string | undefined) => void;
  questionIndex: number;
}

function PreviewQuestion({ questionIndex, activeQuestionId, setActiveQuestionId }: PreviewQuestionProps) {
  const { getAnswer } = useFormAnswersStore();

  const { control } = useFormContext<EditFormType>();

  const questions = useWatch({
    control,
    name: `questions`,
  });

  const languageCode = useWatch({
    control,
    name: 'languageCode'
  });

  const question = questions[questionIndex];

  function meetsDisplayLogicCondition(condition: DisplayLogicCondition | undefined, value: string | undefined, answer: BaseAnswer | undefined): boolean {
    if (condition === undefined || value === undefined) return true;
    if (answer === undefined) return false;

    if (isSingleSelectAnswer(answer)) {
      return condition === 'Includes' && (answer.selection?.optionId === value);
    }

    if (isMultiSelectAnswer(answer)) {
      return condition === 'Includes' && (answer.selection?.some(o => o.optionId === value) ?? false);
    }
    if (isNumberAnswer(answer) || isRatingAnswer(answer)) {
      if (answer.value === undefined) return false;
      const numericValue = +value;
      switch (condition) {
        case 'Equals':
          return answer.value === numericValue;
        case 'NotEquals':
          return answer.value !== numericValue;
        case 'GreaterEqual':
          return answer.value >= numericValue;
        case 'GreaterThan':
          return answer.value > numericValue;
        case 'LessEqual':
          return answer.value <= numericValue;
        case 'LessThan':
          return answer.value < numericValue;
        default:
          return false;
      }
    }

    return false;
  }

  function onPreviousButtonClicked(): void {
    if (questionIndex === 0) {
      return;
    }

    for (let index = questionIndex - 1; index >= 0; index--) {
      const prev = questions[index]!;
      if (prev.hasDisplayLogic) {
        if (meetsDisplayLogicCondition(prev.condition, prev.value, getAnswer(prev.parentQuestionId!))) {
          setActiveQuestionId(prev.questionId);
          break;
        }
      } else {
        setActiveQuestionId(prev.questionId);
        break;
      }
    }
  }

  function onNextButtonClicked(): void {
    if (questionIndex === questions.length - 1) {
      setActiveQuestionId("end");
      return;
    }

    for (let index = questionIndex + 1; index < questions.length; index++) {
      const next = questions[index]!;
      if (next.hasDisplayLogic) {
        if (meetsDisplayLogicCondition(next.condition, next.value, getAnswer(next.parentQuestionId!))) {
          setActiveQuestionId(next.questionId);
          break;
        }
      } else {
        setActiveQuestionId(next.questionId);
        break;
      }
    }
  }

  function resetProgress() {
    setActiveQuestionId(questions[0]?.questionId);
  }

  return (
    <Card className='w-full'>
      <CardHeader className='p-4 bg-slate-600 rounded-t-md'>
        <h3 className='text-white'>Question preview</h3>
      </CardHeader>
      <CardContent>
        {question && (
          <div className='flex flex-col justify-between w-full h-full max-w-lg px-6 pt-6 pb-3'>
            <Button className='mb-4' type='button' onClick={resetProgress}>
              Start from the beginning
            </Button>
            <div>
              {
                question?.$questionType === QuestionType.TextQuestionType && (
                  <PreviewTextQuestion
                    questionId={question.questionId}
                    text={question.text[languageCode]}
                    helptext={question.helptext[languageCode]}
                    inputPlaceholder={question.inputPlaceholder[languageCode]}
                    code={question.code}
                  />)}

              {question?.$questionType === QuestionType.NumberQuestionType && (
                <PreviewNumberQuestion
                  questionId={question.questionId}
                  text={question.text[languageCode]}
                  helptext={question.helptext[languageCode]}
                  inputPlaceholder={question.inputPlaceholder[languageCode]}
                  code={question.code}
                />)}

              {
                question?.$questionType === QuestionType.DateQuestionType && (
                  <PreviewDateQuestion
                    questionId={question.questionId}
                    text={question.text[languageCode]}
                    helptext={question.helptext[languageCode]}
                    code={question.code}
                  />)
              }

              {question?.$questionType === QuestionType.RatingQuestionType && (
                <PreviewRatingQuestion
                  questionId={question.questionId}
                  text={question.text[languageCode]}
                  helptext={question.helptext[languageCode]}
                  scale={question.scale}
                  upperLabel={question.upperLabel[languageCode]}
                  lowerLabel={question.lowerLabel[languageCode]}
                  code={question.code}
                />)}

              {question?.$questionType === QuestionType.MultiSelectQuestionType && (
                <PreviewMultiSelectQuestion
                  questionId={question.questionId}
                  text={question.text[languageCode]}
                  helptext={question.helptext[languageCode]}
                  options={question.options?.map(o => ({ optionId: o.optionId, isFreeText: o.isFreeText, text: o.text[languageCode] })) ?? []}
                  code={question.code}
                />)}

              {question?.$questionType === QuestionType.SingleSelectQuestionType && (
                <PreviewSingleSelectQuestion
                  questionId={question.questionId}
                  text={question.text[languageCode]}
                  helptext={question.helptext[languageCode]}
                  options={question.options?.map(o => ({ optionId: o.optionId, isFreeText: o.isFreeText, text: o.text[languageCode] })) ?? []}
                  code={question.code}
                />)}
            </div>
            <div className='flex justify-end gap-4 mt-4 nav-buttons'>
              <Button
                variant='outline'
                className='flex items-center justify-center gap-2 mb-4'
                type='button'
                onClick={onPreviousButtonClicked}
              >
                <ChevronLeftIcon width={18} />
                Previous
              </Button>
              <Button
                className='flex items-center justify-center gap-2 mb-4'
                type='button'
                onClick={onNextButtonClicked}
              >
                Next
                <ChevronRightIcon width={18} />
              </Button>
            </div>
            {!!questions.length && (
              <div className='mt-8'>
                <Progress value={((questionIndex + 1) / questions.length) * 100} />
              </div>
            )}
          </div>
        )}
        {activeQuestionId === 'end' && (
          <h2 className='p-4 text-2xl text-center'>Finished</h2>
        )}
        {isNilOrWhitespace(activeQuestionId) && (
          <h2 className='p-4 text-2xl text-center'>Select a question</h2>
        )}
      </CardContent>
    </Card>
  );
}
export default PreviewQuestion;