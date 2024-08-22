import { type FunctionComponent, QuestionType } from '@/common/types';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';
import { cn, isNilOrWhitespace } from '@/lib/utils';
import { Draggable } from 'react-beautiful-dnd';
import { useTranslation } from 'react-i18next';

import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { EditFormType } from '@/features/forms/components/EditForm/EditForm';

import { useFormContext, useWatch } from 'react-hook-form';
import { questionsIconMapping } from '../utils';
import DisplayLogicEditor from './DisplayLogicEditor';
import EditNumberQuestion from './EditNumberQuestion';
import EditRatingQuestion from './EditRatingQuestion';
import EditSelectQuestion from './EditSelectQuestion';
import EditTextQuestion from './EditTextQuestion';
import QuestionActions from './QuestionActions';
import type { MoveDirection } from './QuestionsEdit';

interface EditQuestionFactoryProps {
  questionIndex: number;
  activeQuestionId: string | undefined;
  isLastQuestion: boolean;
  setActiveQuestionId: (questionId: string | undefined) => void;
  moveQuestion: (questionIndex: number, direction: MoveDirection) => void;
  duplicateQuestion: (questionIndex: number) => void;
  deleteQuestion: (questionIndex: number) => void;
}

export default function EditQuestionFactory({
  questionIndex,
  activeQuestionId,
  isLastQuestion,
  setActiveQuestionId,
  moveQuestion,
  duplicateQuestion,
  deleteQuestion,
}: EditQuestionFactoryProps): FunctionComponent {
  const { t } = useTranslation();
  const { control, getFieldState } = useFormContext<EditFormType>();


  function getQuestionTypeName(questionType: QuestionType): string {
    switch (questionType) {
      case QuestionType.TextQuestionType: {
        return t('questionEditor.questionType.textQuestion');
      }
      case QuestionType.NumberQuestionType: {
        return t('questionEditor.questionType.numberQuestion');
      }
      case QuestionType.DateQuestionType: {
        return t('questionEditor.questionType.dateQuestion');
      }
      case QuestionType.SingleSelectQuestionType: {
        return t('questionEditor.questionType.singleSelectQuestion');
      }
      case QuestionType.MultiSelectQuestionType: {
        return t('questionEditor.questionType.multiSelectQuestion');
      }
      case QuestionType.RatingQuestionType: {
        return t('questionEditor.questionType.ratingQuestion');
      }
      default: {
        return 'Unknown';
      }
    }
  }

  const question = useWatch({
    control,
    name: `questions.${questionIndex}`,
  });

  const languageCode = useWatch({
    control,
    name: `languageCode`,
  });

  const open = activeQuestionId === question.questionId;

  const IconComponent = questionsIconMapping[question.$questionType] || null;

  return (
    <Draggable draggableId={question.questionId} index={questionIndex}>
      {(provided) => (
        <div
          className={cn(
            open ? 'scale-100 shadow-lg' : 'scale-97 shadow-md',
            'flex flex-row rounded-lg bg-white transition-all duration-300 ease-in-out'
          )}
          ref={provided.innerRef}
          {...provided.draggableProps}
          {...provided.dragHandleProps}>
          <div
            className={cn(
              open ? 'bg-slate-600' : 'bg-purple-900',
              'top-0 w-10 rounded-l-lg p-2 text-center text-sm text-white hover:bg-slate-600',
              !!getFieldState(`questions.${questionIndex}`).invalid && 'bg-red-400 hover:bg-red-600'
            )}>
            {questionIndex + 1}
          </div>
          <Collapsible
            open={open}
            onOpenChange={() => {
              if (activeQuestionId !== question.questionId) {
                setActiveQuestionId(question.questionId);
              } else {
                setActiveQuestionId(undefined);
              }
            }}
            className='flex-1 rounded-r-lg border border-slate-200'>
            <CollapsibleTrigger
              asChild
              className='flex cursor-pointer justify-between p-4 hover:bg-slate-50'>
              <div>
                <div className='inline-flex'>
                  {IconComponent && (
                    <IconComponent className='text-primary -ml-0.5 mr-2 h-5 w-5' aria-hidden='true' />
                  )}
                  <p className='text-sm font-semibold break-all'>
                    {isNilOrWhitespace(question.text[languageCode]) ? getQuestionTypeName(question.$questionType) : question.text[languageCode]}
                  </p>
                </div>

                <div className='flex items-center space-x-2'>
                  <QuestionActions
                    questionIndex={questionIndex}
                    isLastQuestion={isLastQuestion}
                    duplicateQuestion={duplicateQuestion}
                    deleteQuestion={deleteQuestion}
                    moveQuestion={moveQuestion}
                  />
                </div>
              </div>
            </CollapsibleTrigger>
            <CollapsibleContent className='px-4 pb-4 space-y-4'>
              <FormField
                control={control}
                name={`questions.${questionIndex}.text` as const}
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('questionEditor.question.text')}</FormLabel>
                    <FormControl>
                      <Input
                        {...field}
                        {...fieldState}
                        value={field.value[languageCode]}
                        onChange={event => field.onChange({
                          ...field.value,
                          [languageCode]: event.target.value
                        })} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={control}
                name={`questions.${questionIndex}.helptext` as const}
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('questionEditor.question.helptext')}</FormLabel>
                    <FormControl>
                      <Input {...field}
                        {...fieldState}
                        value={field.value[languageCode]}
                        onChange={event => field.onChange({
                          ...field.value,
                          [languageCode]: event.target.value
                        })} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {question.$questionType === QuestionType.TextQuestionType && (
                <EditTextQuestion questionIndex={questionIndex} />
              )}

              {question.$questionType === QuestionType.NumberQuestionType && (
                <EditNumberQuestion questionIndex={questionIndex} />
              )}

              {question.$questionType === QuestionType.RatingQuestionType && (
                <EditRatingQuestion questionIndex={questionIndex} />
              )}

              {(question.$questionType === QuestionType.MultiSelectQuestionType || question.$questionType === QuestionType.SingleSelectQuestionType) && (
                <EditSelectQuestion questionIndex={questionIndex} />
              )}

              <FormField
                control={control}
                name={`questions.${questionIndex}.code` as const}
                render={({ field, fieldState }) => (
                  <FormItem className='w-48'>
                    <FormLabel>{t('questionEditor.question.code')}</FormLabel>
                    <FormControl>
                      <Input {...field} {...fieldState} maxLength={16} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <DisplayLogicEditor questionIndex={questionIndex} />
            </CollapsibleContent>
          </Collapsible>
        </div>
      )}
    </Draggable>
  );
}
