import { BaseAnswer, SingleSelectAnswer, SingleSelectQuestion, AnswerType, SingleSelectAnswerSchema } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../ui/input';
import { Button } from '../ui/button';
import { useTranslation } from 'react-i18next';
import { RadioGroup, RadioGroupItem } from '../ui/radio-group';
import {  useMemo } from 'react';

export interface PreviewSingleSelectQuestionProps {
  languageCode: string;
  question: SingleSelectQuestion;
  isFirstQuestion: boolean;
  isLastQuestion: boolean;
  answer: SingleSelectAnswer;
  onSubmitAnswer: (answer: BaseAnswer) => void;
  onBackButtonClicked: () => void;
}

function PreviewSingleSelectQuestion({
  languageCode,
  question,
  answer,
  isFirstQuestion,
  isLastQuestion,
  onSubmitAnswer,
  onBackButtonClicked }: PreviewSingleSelectQuestionProps) {
  const { t } = useTranslation();

  const form = useForm<SingleSelectAnswer>({
    resolver: zodResolver(SingleSelectAnswerSchema),
    defaultValues: answer ?? {
      questionId: question.id,
      $answerType: AnswerType.SingleSelectAnswerType
    }
  });

  const selectedOptionId = form.watch("selection.optionId");

  const freeTextOptions = useMemo(() => {
    const freeTextOptions: { [optionId: string]: string } = {};
    question.options.filter(o => o.isFreeText).forEach(o => {
      freeTextOptions[o.id] = o.id;
    });

    return freeTextOptions;
  }, [question]);

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmitAnswer)} className="w-2/3 space-y-6">
        <FormField
          control={form.control}
          name="selection.optionId"
          render={({ field }) => (
            <FormItem className="space-y-3">
              <FormLabel>{question.text[languageCode]}</FormLabel>
              <FormControl>
                <RadioGroup
                  onValueChange={field.onChange}
                  defaultValue={field.value}
                  className="flex flex-col space-y-1"
                >
                  {question.options.map(option => (<FormItem className="flex items-center space-x-3 space-y-0" key={option.id}>
                    <FormControl>
                      <RadioGroupItem value={option.id} />
                    </FormControl>
                    <FormLabel className="font-normal">{option.text[languageCode]}</FormLabel>
                  </FormItem>))}
                </RadioGroup>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {!!selectedOptionId && !!freeTextOptions[selectedOptionId] && <FormField
          control={form.control}
          name='selection.text'
          defaultValue=''
          render={({ field }) => (
            <FormItem className='sm:col-span-2'>
              <FormControl>
                <Input {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />}

        <div className="mt-4 flex w-full justify-between">
          {!isFirstQuestion && (
            <Button
              type='button'
              onClick={() => {
                onBackButtonClicked();
              }}
            >
              {t('navigation.button.back')}
            </Button>
          )}
          <div></div>
          <Button type='submit'>
            {isLastQuestion ? t('navigation.button.submit') : t('navigation.button.next')}
          </Button>
        </div>
      </form>
    </Form>
  )
}

export default PreviewSingleSelectQuestion
