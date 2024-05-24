import {
  BaseQuestion,
  DateQuestion,
  MultiSelectQuestion,
  NumberQuestion,
  QuestionType,
  RatingQuestion,
  SingleSelectQuestion,
  TextQuestion,
} from '@/common/types';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';
import { cn } from '@/lib/utils';
import { Draggable } from 'react-beautiful-dnd';
import { useTranslation } from 'react-i18next';

import { MoveDirection } from '../QuestionsEdit';
import EditDateQuestion from './EditDateQuestion';
import EditNumberQuestion from './EditNumberQuestion';
import EditRatingQuestion from './EditRatingQuestion';
import EditSelectQuestion from './EditSelectQuestion';
import EditTextQuestion from './EditTextQuestion';
import QuestionActions from './QuestionActions';
import { useParams } from '@tanstack/react-router';
import { LanguageIcon } from '@heroicons/react/24/outline';

interface EditQuestionFactoryProps {
  formQuestions: BaseQuestion[];
  availableLanguages: string[];
  languageCode: string;
  questionIdx: number;
  activeQuestionId: string | undefined;
  isLastQuestion: boolean;
  isInValid: boolean;
  question: BaseQuestion;
  setActiveQuestionId: (questionId: string | undefined) => void;
  moveQuestion: (questionIndex: number, direction: MoveDirection) => void;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
  duplicateQuestion: (questionIndex: number) => void;
  deleteQuestion: (questionIndex: number) => void;
}

export default function EditQuestionFactory({
  formQuestions,
  availableLanguages,
  languageCode,
  questionIdx,
  activeQuestionId,
  isLastQuestion,
  isInValid,
  question,
  setActiveQuestionId,
  moveQuestion,
  updateQuestion,
  duplicateQuestion,
  deleteQuestion,
}: EditQuestionFactoryProps) {
  const { t } = useTranslation();
  const open = activeQuestionId === question.id;

  const params: any = useParams({
    strict: false,
  });

  function checkIfMissingTranslations() {
    const textRequired = question.text[params['languageCode']] !== '';
    const helpTextRule = question.helptext?.[params['languageCode']]
      ? question.helptext?.[params['languageCode']] !== ''
      : true;
    const genericProperties = textRequired && helpTextRule;

    switch (question.$questionType) {
      case 'numberQuestion' || 'textQuestion':
        return (
          genericProperties && (question as TextQuestion | NumberQuestion).inputPlaceholder?.[params['languageCode']]
        );
      case 'singleSelectQuestion':
        return (
          genericProperties &&
          (question as SingleSelectQuestion).options.every((option) => option.text[params['languageCode']] !== '')
        );
      default:
        return genericProperties;
    }
  }

  function getQuestionTypeName(questionType: QuestionType): string {
    switch (questionType) {
      case QuestionType.TextQuestionType:
        return t('questionEditor.questionType.textQuestion');
      case QuestionType.NumberQuestionType:
        return t('questionEditor.questionType.numberQuestion');
      case QuestionType.DateQuestionType:
        return t('questionEditor.questionType.dateQuestion');
      case QuestionType.SingleSelectQuestionType:
        return t('questionEditor.questionType.singleSelectQuestion');
      case QuestionType.MultiSelectQuestionType:
        return t('questionEditor.questionType.multiSelectQuestion');
      case QuestionType.RatingQuestionType:
        return t('questionEditor.questionType.ratingQuestion');
    }

    return 'Unknown';
  }

  return (
    <Draggable draggableId={question.id} index={questionIdx}>
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
              isInValid && 'bg-red-400  hover:bg-red-600'
            )}>
            {questionIdx + 1}
          </div>
          <Collapsible
            open={open}
            onOpenChange={() => {
              if (activeQuestionId !== question.id) {
                setActiveQuestionId(question.id);
              } else {
                setActiveQuestionId(undefined);
              }
            }}
            className='flex-1 rounded-r-lg border border-slate-200'>
            <CollapsibleTrigger
              asChild
              className={cn(open ? '' : '  ', 'flex cursor-pointer justify-between p-4 hover:bg-slate-50')}>
              <div>
                <div className='inline-flex'>
                  <div>
                    {!!params['languageCode'] &&
                      (!checkIfMissingTranslations() ? (
                        <div className='flex items-center 	 rounded-md text-yellow-700 bg-yellow-100 p-2 mb-2'>
                          <LanguageIcon width={22} />
                          This question is missing translations.
                        </div>
                      ) : (
                        <div className='flex gap-2 	 items-center rounded-md text-green-700 bg-green-100 p-2 mb-2'>
                          <LanguageIcon width={22} />
                          This question is translated.
                        </div>
                      ))}
                    <p className='text-sm font-semibold'>
                      {question.code} -{' '}
                      {(params['languageCode'] ? question.text[params['languageCode']] : question.text[languageCode]) ||
                        getQuestionTypeName(question.$questionType)}
                    </p>
                  </div>
                </div>

                <div className='flex items-center space-x-2'>
                  <QuestionActions
                    questionIdx={questionIdx}
                    isLastQuestion={isLastQuestion}
                    duplicateQuestion={duplicateQuestion}
                    deleteQuestion={deleteQuestion}
                    moveQuestion={moveQuestion}
                    disabled={!!params['languageCode']}
                  />
                </div>
              </div>
            </CollapsibleTrigger>
            <CollapsibleContent className='px-4 pb-4'>
              {question.$questionType === QuestionType.TextQuestionType ? (
                <EditTextQuestion
                  formQuestions={formQuestions}
                  availableLanguages={availableLanguages}
                  languageCode={languageCode}
                  question={question as TextQuestion}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.DateQuestionType ? (
                <EditDateQuestion
                  formQuestions={formQuestions}
                  availableLanguages={availableLanguages}
                  languageCode={languageCode}
                  question={question as DateQuestion}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.NumberQuestionType ? (
                <EditNumberQuestion
                  formQuestions={formQuestions}
                  availableLanguages={availableLanguages}
                  languageCode={languageCode}
                  question={question as NumberQuestion}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.MultiSelectQuestionType ? (
                <EditSelectQuestion
                  formQuestions={formQuestions}
                  availableLanguages={availableLanguages}
                  languageCode={languageCode}
                  question={question as MultiSelectQuestion}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.SingleSelectQuestionType ? (
                <EditSelectQuestion
                  formQuestions={formQuestions}
                  availableLanguages={availableLanguages}
                  languageCode={languageCode}
                  question={question as SingleSelectQuestion}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : question.$questionType === QuestionType.RatingQuestionType ? (
                <EditRatingQuestion
                  formQuestions={formQuestions}
                  availableLanguages={availableLanguages}
                  languageCode={languageCode}
                  question={question as RatingQuestion}
                  questionIdx={questionIdx}
                  updateQuestion={updateQuestion}
                  isInValid={isInValid}
                />
              ) : null}
            </CollapsibleContent>
          </Collapsible>
        </div>
      )}
    </Draggable>
  );
}
