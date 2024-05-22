import { NumberAnswer, NumberQuestion } from '@/common/types';
import { Input } from '@/components/ui/input';
import { Description, Field, Label } from '@/components/ui/fieldset';
import { useParams } from '@tanstack/react-router';
export interface PreviewNumberQuestionProps {
  languageCode: string;
  question: NumberQuestion;
  answer: NumberAnswer;
  setAnswer: (answer: NumberAnswer) => void;
}

function PreviewNumberQuestion({ languageCode, question, answer, setAnswer }: PreviewNumberQuestionProps) {
  const params: any = useParams({
    strict: false,
  });
  return (
    <Field>
      <Label>
        {question.code} - {question.text[params['languageCode'] ? params['languageCode'] : languageCode]}
      </Label>
      {!!question.helptext && (
        <Description>{question.helptext[params['languageCode'] ? params['languageCode'] : languageCode]}</Description>
      )}
      <Input
        type='number'
        placeholder={
          question.inputPlaceholder
            ? question.inputPlaceholder[params['languageCode'] ? params['languageCode'] : languageCode]
            : ''
        }
      />
    </Field>
  );
}

export default PreviewNumberQuestion;
