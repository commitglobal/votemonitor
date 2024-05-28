import { TextAnswer, TextQuestion } from '@/common/types';
import { Description, Field, Label } from '@/components/ui/fieldset';
import { Textarea } from '../../ui/textarea';
import { useParams } from '@tanstack/react-router';
import { Input } from '@/components/ui/input';

export interface PreviewTextQuestionProps {
  languageCode: string;
  question: TextQuestion;
  answer?: TextAnswer;
  setAnswer: (answer: TextAnswer) => void;
}

function PreviewTextQuestion({ languageCode, question, answer, setAnswer }: PreviewTextQuestionProps) {
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
      <Textarea
        placeholder={
          question.inputPlaceholder
            ? question.inputPlaceholder[params['languageCode'] ? params['languageCode'] : languageCode]
            : ''
        }
      />
    </Field>
  );
}

export default PreviewTextQuestion;
