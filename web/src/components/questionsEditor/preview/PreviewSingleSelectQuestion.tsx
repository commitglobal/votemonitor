import { isSingleSelectAnswer } from '@/common/guards';
import { AnswerType, SingleSelectAnswer } from '@/common/types';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { useEffect, useMemo, useState } from 'react';
import { RadioGroup, RadioGroupItem } from '../../ui/radio-group';
import { useFormAnswersStore } from '../answers-store';

export interface PreviewSingleSelectQuestionProps {
  questionId: string;
  text?: string;
  helptext?: string;
  options: { optionId: string, text?: string, isFreeText: boolean }[];
  code: string;
}

function PreviewSingleSelectQuestion({ code, questionId, text, helptext, options }: PreviewSingleSelectQuestionProps) {
  const { setAnswer, getAnswer } = useFormAnswersStore();
  const [localAnswer, setLocalAnswer] = useState<SingleSelectAnswer | undefined>(undefined)

  const regularOptions = useMemo(() => {
    if (!options) {
      return [];
    }
    const regularOptions = options.filter((option) => !option.isFreeText);
    return regularOptions;
  }, [options]);

  // Currently we only support one free text option
  const freeTextOption = useMemo(() => options.find((option) => option.isFreeText), [options]);

  useEffect(() => {
    const singleSelectAnswer = getAnswer(questionId) as SingleSelectAnswer;
    setLocalAnswer(singleSelectAnswer);
  }, [questionId]);

  useEffect(() => {
    const singleSelectAnswer = getAnswer(questionId);
    if (singleSelectAnswer && isSingleSelectAnswer(singleSelectAnswer)) {
      setLocalAnswer(singleSelectAnswer);
    } else {
      const multiSelectAnswer: SingleSelectAnswer = { $answerType: AnswerType.SingleSelectAnswerType, questionId };
      setAnswer(multiSelectAnswer);
      setLocalAnswer(multiSelectAnswer);
    }
  }, [questionId]);

  return (
    <div className="grid gap-2">
      <Label htmlFor={`${questionId}-value`} className='font-semibold'>{code + ' - '}{text}</Label>
      <Label htmlFor={`${questionId}-value`} className='text-sm italic'>{helptext}</Label>
      <RadioGroup
        onValueChange={(value) => {
          const newAnswer: SingleSelectAnswer = { $answerType: AnswerType.SingleSelectAnswerType, questionId, selection: { optionId: value, text: '' } };
          setAnswer(newAnswer);
          setLocalAnswer(newAnswer);
        }}
        value={localAnswer?.selection?.optionId}>
        {regularOptions?.map((option) => (
          <div className="flex items-center space-x-2" key={option.optionId}>
            <RadioGroupItem value={option.optionId} id={option.optionId} />
            <Label htmlFor={option.optionId}>{option.text}</Label>
          </div>
        ))}
        {!!freeTextOption && (
          <div className="flex items-center space-x-2" key={freeTextOption.optionId}>
            <RadioGroupItem value={freeTextOption.optionId} id={freeTextOption.optionId} />
            <Label htmlFor={freeTextOption.optionId}>{freeTextOption.text}</Label>
          </div>
        )}
      </RadioGroup>
      {(!!localAnswer?.selection?.optionId && localAnswer?.selection?.optionId === freeTextOption?.optionId) && (
        <Textarea onChange={(e) => {
          const newAnswer: SingleSelectAnswer = { ...localAnswer!, selection: { ...localAnswer!.selection, text: e.target.value } };
          setAnswer(newAnswer);
          setLocalAnswer(newAnswer);
        }}
          value={localAnswer?.selection?.text ?? ''} />
      )}
    </div>
  );
}

export default PreviewSingleSelectQuestion;
