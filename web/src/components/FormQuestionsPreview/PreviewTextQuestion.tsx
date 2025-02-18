import { Label } from '@/components/ui/label';
import { Textarea } from '../ui/textarea';
import { AnswerType, TextAnswer } from '@/common/types';
import { useFormAnswersStore } from '../questionsEditor/answers-store';
import { useEffect, useState } from 'react';

export interface PreviewTextQuestionProps {
  questionId: string;
  text?: string;
  helptext?: string;
  inputPlaceholder?: string;
  code: string;
}

function PreviewTextQuestion({ code, questionId, text, helptext, inputPlaceholder }: PreviewTextQuestionProps) {
  const { setAnswer, getAnswer } = useFormAnswersStore();
  const [localAnswer, setLocalAnswer] = useState<TextAnswer | undefined>(undefined);
  useEffect(() => {
    setLocalAnswer(getAnswer(questionId) as TextAnswer);
  }, [questionId]);

  return (
    <div className='grid gap-6'>
      <div className='grid gap-2'>
        <Label htmlFor={`${questionId}-value`} className='font-semibold break-all'>
          {code + ' - '}
          {text}
        </Label>
        <Label htmlFor={`${questionId}-value`} className='text-sm italic'>
          {helptext}
        </Label>
        <Textarea
          id={`${questionId}-value`}
          placeholder={inputPlaceholder}
          onChange={(e) => {
            const textAnswer: TextAnswer = { $answerType: AnswerType.TextAnswerType, questionId, text: e.target.value };
            setAnswer(textAnswer);
          }}
          value={localAnswer?.text}
        />
      </div>
    </div>
  );
}

export default PreviewTextQuestion;
