import { AnswerType, BaseAnswer, NumberAnswer, NumberAnswerSchema, NumberQuestion } from '@/common/types'
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '@/components/ui/input';
import { useTranslation } from 'react-i18next';
import { Description, Field, Label } from '@/components/ui/fieldset';
export interface PreviewNumberQuestionProps {
  languageCode: string;
  question: NumberQuestion;
  answer: NumberAnswer;
  isFirstQuestion: boolean;
  isLastQuestion: boolean;
  onSubmitAnswer: (answer: BaseAnswer) => void;
  onBackButtonClicked: () => void;
}

function PreviewNumberQuestion({
  languageCode,
  question,
  answer,
  isFirstQuestion,
  isLastQuestion,
  onSubmitAnswer,
  onBackButtonClicked }: PreviewNumberQuestionProps) {
  const { t } = useTranslation();

  const form = useForm<NumberAnswer>({
    resolver: zodResolver(NumberAnswerSchema),
    defaultValues: answer ?? {
      questionId: question.id,
      $answerType: AnswerType.NumberAnswerType,
    }
  });

  return (
    <Field>
      <Label>{question.code} - {question.text[languageCode]}</Label>
      {!!question.helptext && <Description>{question.helptext[languageCode]}</Description>}
      <Input type="number" placeholder={question.inputPlaceholder ? question.inputPlaceholder[languageCode] : ''} />
    </Field>
  )
}

export default PreviewNumberQuestion
