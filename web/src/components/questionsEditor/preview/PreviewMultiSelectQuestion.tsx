import { useEffect, useMemo, useState } from 'react';

import { isMultiSelectAnswer } from '@/common/guards';
import { AnswerType, MultiSelectAnswer, SelectedOption } from '@/common/types';
import { Checkbox, CheckboxField, CheckboxGroup } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { useFormAnswersStore } from '../answers-store';

export interface PreviewMultiSelectQuestionProps {
  questionId: string;
  text?: string;
  helptext?: string;
  options: { optionId: string, text?: string, isFreeText: boolean }[];
  code: string;
}

function PreviewMultiSelectQuestion({ code, questionId, text, helptext, options }: PreviewMultiSelectQuestionProps) {
  const { setAnswer, getAnswer } = useFormAnswersStore();
  const [localAnswer, setLocalAnswer] = useState<MultiSelectAnswer | undefined>(undefined)

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
    const multiSelectAnswer = getAnswer(questionId);
    if (multiSelectAnswer && isMultiSelectAnswer(multiSelectAnswer)) {
      setLocalAnswer(multiSelectAnswer);
    } else {
      const multiSelectAnswer: MultiSelectAnswer = { $answerType: AnswerType.MultiSelectAnswerType, questionId, selection: [] };
      setAnswer(multiSelectAnswer);
      setLocalAnswer(multiSelectAnswer);
    }
  }, [questionId]);


  const handleOptionSelected = (isChecked: boolean, optionId: string) => {
    let newAnswer: MultiSelectAnswer;
    if (isChecked) {
      newAnswer = {
        questionId: questionId,
        $answerType: AnswerType.MultiSelectAnswerType,
        selection: [
          ...localAnswer?.selection ?? [],
          { optionId: optionId }
        ]
      }


    } else {
      newAnswer = {
        questionId: questionId,
        $answerType: AnswerType.MultiSelectAnswerType,
        selection: [
          ...localAnswer?.selection?.filter(o => o.optionId !== optionId) ?? []
        ]
      }
    }

    setAnswer(newAnswer);
    setLocalAnswer(newAnswer);
  };

  return (
    <div className="grid gap-2">
      <Label htmlFor={`${questionId}-value`} className='font-semibold'>{code + ' - '}{text}</Label>
      <Label htmlFor={`${questionId}-value`} className='text-sm italic'>{helptext}</Label>
      <CheckboxGroup>
        {regularOptions.map((option) => (
          <CheckboxField key={option.optionId}>
            <Checkbox
              name={option.optionId}
              checked={localAnswer?.selection?.some(o => o.optionId === option.optionId)}
              onChange={(checked) => handleOptionSelected(checked, option.optionId)} />
            <Label>{option.text}</Label>
          </CheckboxField>
        ))}

        {!!freeTextOption && (
          <CheckboxField>
            <Checkbox
              name={freeTextOption.optionId}
              checked={localAnswer?.selection?.some(o => o.optionId === freeTextOption.optionId)}
              onChange={(checked) => {
                handleOptionSelected(checked, freeTextOption.optionId);
              }}
            />
            <Label>{freeTextOption.text}</Label>
          </CheckboxField>
        )}
      </CheckboxGroup>
      {localAnswer?.selection?.some(o => o.optionId === freeTextOption?.optionId) && (
        <Textarea
          onChange={(e) => {
            const option: SelectedOption = {
              optionId: freeTextOption!.optionId,
              text: e.target.value
            };

            const newAnswer: MultiSelectAnswer = {
              ...localAnswer!,
              selection: [
                ...localAnswer?.selection?.filter(o => o.optionId === freeTextOption?.optionId) ?? [],
                option
              ]
            };
            setAnswer(newAnswer);
            setLocalAnswer(newAnswer);
          }}
          value={localAnswer?.selection?.find(o => o.optionId === freeTextOption?.optionId)?.text ?? ''}
          defaultValue='' />
      )}
    </div>
  );
}

export default PreviewMultiSelectQuestion;
