import { AnswerType, type FunctionComponent, type RatingAnswer, type RatingScaleType } from '@/common/types';
import { Label } from '@/components/ui/label';
import { RatingGroup } from '@/components/ui/ratings';
import { ratingScaleToNumber } from '@/lib/utils';
import { useFormAnswersStore } from '../answers-store';
import { useState, useEffect } from 'react';

export interface PreviewRatingQuestionProps {
  questionId: string;
  text?: string;
  helptext?: string;
  scale: RatingScaleType;
  lowerLabel?: string;
  upperLabel?: string;
  code: string;
}

function PreviewRatingQuestion({ code, questionId, text, helptext, scale, lowerLabel, upperLabel }: PreviewRatingQuestionProps): FunctionComponent {
  const { setAnswer, getAnswer } = useFormAnswersStore();
  const [localAnswer, setLocalAnswer] = useState<RatingAnswer | undefined>(undefined)

  useEffect(() => {
    const ratingAnswer = getAnswer(questionId) as RatingAnswer;
    setLocalAnswer(ratingAnswer);
  }, [questionId]);

  return (
    <div className="grid gap-6">
      <div className="grid gap-2">
        <Label htmlFor={`${questionId}-value`} className='font-semibold'>{code + ' - '}{text}</Label>
        <Label htmlFor={`${questionId}-value`} className='text-sm italic'>{helptext}</Label>
        <RatingGroup
          scale={ratingScaleToNumber(scale)}
          id={`${questionId}-value`}
          lowerLabel={lowerLabel}
          upperLabel={upperLabel}
          defaultValue={localAnswer?.value?.toString()}
          onValueChange={(value) => {
            const ratingAnswer: RatingAnswer = { $answerType: AnswerType.RatingAnswerType, questionId, value: Number(value) };
            setAnswer(ratingAnswer);
          }} />
      </div>
    </div>
  );
}

export default PreviewRatingQuestion;
