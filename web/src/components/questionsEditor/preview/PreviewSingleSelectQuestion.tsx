import { BaseAnswer, SingleSelectAnswer, SingleSelectQuestion, AnswerType, SingleSelectAnswerSchema } from '@/common/types'
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../../ui/input';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';
import { RadioGroup, RadioField, Radio } from '../../ui/radio-group';
import { useMemo, useState } from 'react';
import { Description, Field, Fieldset, Label, Legend } from '@/components/ui/fieldset'
import { Text } from '@/components/ui/text'
import { Textarea } from '@/components/ui/textarea';

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
    <Fieldset>
      <Legend>{question.code} - {question.text[languageCode]}</Legend>
      {!!question.helptext && <Text>{question.helptext[languageCode]}</Text>}
      <RadioGroup onChange={(value) => {
        setFreeTextSelected(value === freeTextOption?.id);
      }}>
        {regularOptions?.map(option => (
          <RadioField key={option.id}>
            <Radio value={option.id} />
            <Label>{option.text[languageCode]}</Label>
          </RadioField>
        ))
        }
        {!!freeTextOption && <RadioField>
          <Radio value={freeTextOption.id} />
          <Label>{freeTextOption.text[languageCode]}</Label>
        </RadioField>}
      </RadioGroup>
      {freeTextSelected && <Field><Textarea /></Field>}
    </Fieldset>
  )
}

export default PreviewSingleSelectQuestion
