import type { FunctionComponent, RatingAnswer, RatingQuestion } from '@/common/types';
import { RatingGroup } from '../../ui/ratings';
import { Description, Field, Label } from '@/components/ui/fieldset';
import { ratingScaleToNumber } from '@/lib/utils';

export interface PreviewRatingQuestionProps {
  languageCode: string;
  question: RatingQuestion;
  answer: RatingAnswer;
  setAnswer: (answer: RatingAnswer) => void;
}

function PreviewRatingQuestion({ languageCode, question }: PreviewRatingQuestionProps): FunctionComponent {
  return (
    <Field>
      <Label>
        {question.code} - {question.text[languageCode]}
      </Label>
      {!!question.helptext && <Description>{question.helptext[languageCode]}</Description>}
      <RatingGroup className='my-2' scale={ratingScaleToNumber(question.scale)} name='value' />
    </Field>
  );
}

export default PreviewRatingQuestion;
