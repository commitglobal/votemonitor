import { NumberAnswer, NumberQuestion } from '@/common/types'
import { Input } from '@/components/ui/input';
import { Description, Field, Label } from '@/components/ui/fieldset';
export interface PreviewNumberQuestionProps {
  languageCode: string;
  question: NumberQuestion;
  answer: NumberAnswer;
  setAnswer: (answer: NumberAnswer) => void;
}

function PreviewNumberQuestion({
  languageCode,
  question,
  answer,
  setAnswer }: PreviewNumberQuestionProps) {
  return (
    <Field>
      <Label>{question.code} - {question.text[languageCode]}</Label>
      {!!question.helptext && <Description>{question.helptext[languageCode]}</Description>}
      <Input type="number" placeholder={question.inputPlaceholder ? question.inputPlaceholder[languageCode] : ''} />
    </Field>
  )
}

export default PreviewNumberQuestion
