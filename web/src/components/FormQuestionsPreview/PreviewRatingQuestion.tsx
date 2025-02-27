import { AnswerType, type FunctionComponent, type RatingAnswer, type RatingScaleType } from '@/common/types';
import { Label } from '@/components/ui/label';
import { RatingGroup } from '@/components/ui/ratings';
import { ratingScaleToNumber } from '@/lib/utils';
import { useFormAnswersStore } from '../questionsEditor/answers-store';

export interface PreviewRatingQuestionProps {
  questionId: string;
  text?: string;
  helptext?: string;
  scale: RatingScaleType;
  lowerLabel?: string;
  upperLabel?: string;
  code: string;
}

function PreviewRatingQuestion({
  code,
  questionId,
  text,
  helptext,
  scale,
  lowerLabel,
  upperLabel,
}: PreviewRatingQuestionProps): FunctionComponent {
  const { setAnswer, getAnswer } = useFormAnswersStore();

  const value = (getAnswer(questionId) as RatingAnswer)?.value?.toString();

  return (
    <div className='grid gap-6'>
      <div className='grid gap-2'>
        <Label htmlFor={`${questionId}-value`} className='font-semibold break-all'>
          {code + ' - '}
          {text}
        </Label>
        <Label htmlFor={`${questionId}-value`} className='text-sm italic break-all'>
          {helptext}
        </Label>
        <RatingGroup
          scale={ratingScaleToNumber(scale)}
          id={`${questionId}-value`}
          lowerLabel={lowerLabel}
          upperLabel={upperLabel}
          value={value}
          onValueChange={(value) => {
            const ratingAnswer: RatingAnswer = {
              $answerType: AnswerType.RatingAnswerType,
              questionId,
              value: Number(value),
            };
            setAnswer(ratingAnswer);
          }}
        />
      </div>
    </div>
  );
}

export default PreviewRatingQuestion;
