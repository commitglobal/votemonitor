import { AnswerType, NumberAnswer } from '@/common/types';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useFormAnswersStore } from '../questionsEditor/answers-store';

export interface PreviewNumberQuestionProps {
  questionId: string;
  text?: string;
  helptext?: string;
  inputPlaceholder?: string;
  code?: string;
}

function PreviewNumberQuestion({ code, questionId, text, helptext, inputPlaceholder }: PreviewNumberQuestionProps) {
  const { setAnswer, getAnswer } = useFormAnswersStore();

  const value = (getAnswer(questionId) as NumberAnswer)?.value?.toString();

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
        <Input
          id={`${questionId}-value`}
          placeholder={inputPlaceholder}
          type='number'
          value={value}
          onChange={(e) => {
            const numberAnswer: NumberAnswer = {
              $answerType: AnswerType.NumberAnswerType,
              questionId,
              value: Number(e.target.value),
            };
            setAnswer(numberAnswer);
          }}
          min='0'
        />
      </div>
    </div>
  );
}

export default PreviewNumberQuestion;
