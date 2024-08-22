import { type FunctionComponent, QuestionType } from '@/common/types';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';
import { cn, isNilOrWhitespace } from '@/lib/utils';
import { useTranslation } from 'react-i18next';

import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';

import { EditFormTranslationType } from '@/features/forms/components/EditFormTranslation/EditFormTranslation';
import { LanguageIcon } from '@heroicons/react/24/outline';
import { useFormContext, useWatch } from 'react-hook-form';
import { questionsIconMapping } from '../utils';
import TranslateNumberQuestion from './TranslateNumberQuestion';
import TranslateRatingQuestion from './TranslateRatingQuestion';
import TranslateSelectQuestion from './TranslateSelectQuestion';
import TranslateTextQuestion from './TranslateTextQuestion';

interface TranslateQuestionFactoryProps {
  questionIndex: number;
  activeQuestionId: string | undefined;
  setActiveQuestionId: (questionId: string | undefined) => void;
}

export default function TranslateQuestionFactory({
  questionIndex,
  activeQuestionId,
  setActiveQuestionId,
}: TranslateQuestionFactoryProps): FunctionComponent {
  const { t } = useTranslation();
  const { control, getFieldState } = useFormContext<EditFormTranslationType>();

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

  const languageCode = useWatch({
    control,
    name: `languageCode`,
  });

  const defaultLanguageCode = useWatch({
    control,
    name: `defaultLanguage`,
  });

  const question = useWatch({
    control,
    name: `questions.${questionIndex}`,
  });

  const questionState = getFieldState(`questions.${questionIndex}`);

  const open = activeQuestionId === question.questionId;

  const IconComponent = questionsIconMapping[question.$questionType] || null;

  return (
    <div
      className={cn(
        open ? 'scale-100 shadow-lg' : 'scale-97 shadow-md',
        'flex flex-row rounded-lg bg-white transition-all duration-300 ease-in-out'
      )}>
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
          if (!open) {
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
            {(!questionState.invalid ? (
              <div className='flex gap-2 items-center rounded-md text-green-700 bg-green-100 p-2 mb-2'>
                <LanguageIcon width={22} />
                This question is translated.
              </div>
            ) : (
              <div className='flex items-center rounded-md text-yellow-700 bg-yellow-100 p-2 mb-2'>
                <LanguageIcon width={22} />
                This question is missing translations.
              </div>
            ))}

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
                    placeholder={field.value[defaultLanguageCode]}
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
                  <Input
                    {...field}
                    {...fieldState}
                    value={field.value[languageCode]}
                    placeholder={field.value[defaultLanguageCode]}
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
            <TranslateTextQuestion questionIndex={questionIndex} />
          )}

          {question.$questionType === QuestionType.NumberQuestionType && (
            <TranslateNumberQuestion questionIndex={questionIndex} />
          )}

          {question.$questionType === QuestionType.RatingQuestionType && (
            <TranslateRatingQuestion questionIndex={questionIndex} />
          )}

          {(question.$questionType === QuestionType.MultiSelectQuestionType || question.$questionType === QuestionType.SingleSelectQuestionType) && (
            <TranslateSelectQuestion questionIndex={questionIndex} />
          )}

          <FormField
            control={control}
            name={`questions.${questionIndex}.code` as const}
            render={({ field }) => (
              <FormItem className='w-48'>
                <FormLabel>{t('questionEditor.question.code')}</FormLabel>
                <FormControl>
                  <Input {...field} disabled />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </CollapsibleContent>
      </Collapsible>
    </div>

  );
}
