import type { FunctionComponent, RatingAnswer, RatingQuestion } from '@/common/types';
import { RatingGroup } from '../../ui/ratings';
import { Description, Field, Label } from '@/components/ui/fieldset';
import { ratingScaleToNumber } from '@/lib/utils';
import { useParams } from '@tanstack/react-router';

export interface PreviewRatingQuestionProps {
  languageCode: string;
  question: RatingQuestion;
  answer: RatingAnswer;
  setAnswer: (answer: RatingAnswer) => void;
}

function PreviewRatingQuestion({ languageCode, question }: PreviewRatingQuestionProps): FunctionComponent {
  const params: any = useParams({
    strict: false,
  });
  return (
    <Field className='flex flex-col'>
      <Label>
        {question.code} - {question.text[params['languageCode'] ? params['languageCode'] : languageCode]}
      </Label>
      {!!question.helptext && (
        <Description>{question.helptext[params['languageCode'] ? params['languageCode'] : languageCode]}</Description>
      )}
      <RatingGroup className='my-2' scale={ratingScaleToNumber(question.scale)} name='value' />
    </Field>
  );
}

export default PreviewRatingQuestion;
