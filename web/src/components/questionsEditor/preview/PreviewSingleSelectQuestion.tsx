import { BaseAnswer, SingleSelectAnswer, SingleSelectQuestion, AnswerType, SingleSelectAnswerSchema } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../../ui/input';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';
import { RadioGroup, RadioGroupItem } from '../../ui/radio-group';
import { useMemo, useState } from 'react';

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
      $answerType: AnswerType.SingleSelectAnswerType,
      selection: {
        text: ''
      }
    }
  });

  const [freeTextSelected, setFreeTextSelected] = useState(
    !!answer && !question.options.find((c) => c.id === answer.selection?.optionId && c.isFreeText === true)
  );

  const regularOptions = useMemo(() => {
    if (!question.options) {
      return [];
    }
    const regularOptions = question.options.filter((option) => !option.isFreeText);
    return regularOptions;
  }, [question.options]);

  // Currently we only support one free text option
  const freeTextOption = useMemo(
    () => question.options.find((option) => option.isFreeText),
    [question.options]
  );

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
                  onValueChange={(value) => {
                    field.onChange(value);
                    setFreeTextSelected(value === freeTextOption?.id);
                  }}
                  defaultValue={field.value}
                  className="flex flex-col space-y-1"
                >
                  {regularOptions?.map(option => (<FormItem className="flex items-center space-x-3 space-y-0" key={option.id}>
                    <FormControl>
                      <RadioGroupItem value={option.id} />
                    </FormControl>
                    <FormLabel className="font-normal">{option.text[languageCode]}</FormLabel>
                  </FormItem>))}

                  {!!freeTextOption &&
                    <FormItem className="flex items-center space-x-3 space-y-0" key={freeTextOption.id}>
                      <FormControl>
                        <RadioGroupItem value={freeTextOption.id} />
                      </FormControl>
                      <FormLabel className="font-normal">{freeTextOption.text[languageCode]}</FormLabel>
                    </FormItem>
                  }
                </RadioGroup>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        {freeTextSelected &&
          <FormField
            control={form.control}
            name="selection.text"
            render={({ field }) => (
              <FormItem>
                <FormControl>
                  <Input {...field} placeholder={t("app.input.pleaseSpecify")}/>
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />}
      </form>
    </Form>
  )
}

export default PreviewSingleSelectQuestion
