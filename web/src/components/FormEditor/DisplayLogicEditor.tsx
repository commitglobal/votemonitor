import { EditQuestionType } from '@/common/form-requests';
import {
  DisplayLogicCondition,
  QuestionType
} from '@/common/types';
import { Collapsible, CollapsibleContent } from '@/components/ui/collapsible';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Switch } from '@/components/ui/switch';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { EditFormType } from '@/components/FormEditor/FormEditor';
import { ratingScaleToNumber } from '@/lib/utils';
import { InformationCircleIcon } from '@heroicons/react/24/outline';
import { useEffect, useState } from 'react';
import { useFormContext, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';

interface DisplayLogicEditorProps {
  questionIndex: number;
}

const conditions: {
  [questionType: string]: DisplayLogicCondition[];
} = {
  multiSelectQuestion: ['Includes'],
  singleSelectQuestion: ['Includes'],
  numberQuestion: ['Equals', 'NotEquals', 'LessThan', 'LessEqual', 'GreaterThan', 'GreaterEqual'],
  ratingQuestion: ['Equals', 'NotEquals', 'LessThan', 'LessEqual', 'GreaterThan', 'GreaterEqual'],
};

export default function DisplayLogicEditor({ questionIndex }: DisplayLogicEditorProps) {
  const { t } = useTranslation();
  const [hasDisplayLogic, setHasDisplayLogic] = useState(false);
  const [question, setQuestion] = useState<EditQuestionType | undefined>(undefined);
  const [parentQuestion, setParentQuestion] = useState<EditQuestionType | undefined>(undefined);
  const [availableParentQuestions, setAvailableParentQuestions] = useState<EditQuestionType[]>([]);

  const { control, setValue, register, trigger } = useFormContext<EditFormType>();

  const languageCode = useWatch({
    control,
    name: `languageCode`,
  });

  const questions = useWatch({
    control,
    name: `questions`,
    defaultValue: []
  });

  const parentQuestionId = useWatch({
    control,
    name: `questions.${questionIndex}.parentQuestionId`,
    defaultValue: undefined
  });

  register(`questions.${questionIndex}.hasDisplayLogic`);
  register(`questions.${questionIndex}.parentQuestionId`);
  register(`questions.${questionIndex}.value`);
  register(`questions.${questionIndex}.condition`);


  useEffect(() => {
    setAvailableParentQuestions(
      questions
        ?.slice(0, questionIndex)
        ?.filter(
          (q) =>
            q.$questionType === QuestionType.SingleSelectQuestionType ||
            q.$questionType === QuestionType.MultiSelectQuestionType ||
            q.$questionType === QuestionType.RatingQuestionType ||
            q.$questionType === QuestionType.NumberQuestionType
        ) ?? []
    );

    setQuestion(questions[questionIndex]);
    setHasDisplayLogic(questions[questionIndex]!.hasDisplayLogic);
  }, [questions, questionIndex]);

  useEffect(() => {
    setParentQuestion(
      questions
        ?.slice(0, questionIndex)
        ?.filter(
          (q) =>
            q.$questionType === QuestionType.SingleSelectQuestionType ||
            q.$questionType === QuestionType.MultiSelectQuestionType ||
            q.$questionType === QuestionType.RatingQuestionType ||
            q.$questionType === QuestionType.NumberQuestionType
        )
        ?.find(q => q.questionId === parentQuestionId)
    );
  }, [questions, questionIndex, parentQuestionId]);


  function handleHasDisplayLogicChanged(value: boolean) {
    setHasDisplayLogic(value);
    setValue(`questions.${questionIndex}.hasDisplayLogic`, value, { shouldDirty: true, shouldTouch: true, shouldValidate: true });
    handleParentQuestionSelected(availableParentQuestions[0]?.questionId!);
    trigger(`questions.${questionIndex}`)
  }

  function handleParentQuestionSelected(questionId: string) {
    const parentQuestion = availableParentQuestions.find((q) => q.questionId === questionId)!;
    setValue(`questions.${questionIndex}.parentQuestionId`, questionId, { shouldDirty: true, shouldTouch: true, shouldValidate: true });

    if (parentQuestion?.$questionType === QuestionType.RatingQuestionType) {
      setValue(`questions.${questionIndex}.condition`, 'Equals', { shouldDirty: true, shouldTouch: true, shouldValidate: true });
      setValue(`questions.${questionIndex}.value`, '1', { shouldDirty: true, shouldTouch: true, shouldValidate: true });
    }

    if (parentQuestion?.$questionType === QuestionType.NumberQuestionType) {
      setValue(`questions.${questionIndex}.condition`, 'Equals', { shouldDirty: true, shouldTouch: true, shouldValidate: true });
      setValue(`questions.${questionIndex}.value`, '0', { shouldDirty: true, shouldTouch: true, shouldValidate: true });
    }

    if (
      parentQuestion?.$questionType === QuestionType.SingleSelectQuestionType ||
      parentQuestion?.$questionType === QuestionType.MultiSelectQuestionType
    ) {
      const optionId = parentQuestion!.options[0]?.optionId;
      setValue(`questions.${questionIndex}.condition`, 'Includes', { shouldDirty: true, shouldTouch: true, shouldValidate: true });
      setValue(`questions.${questionIndex}.value`, optionId, { shouldDirty: true, shouldTouch: true, shouldValidate: true });
    }
  }

  function handleConditionChanged(condition: DisplayLogicCondition) {
    if (!!condition) {
      setValue(`questions.${questionIndex}.condition`, condition, { shouldDirty: true, shouldTouch: true, shouldValidate: true });
    }
  }

  function handleValueChanged(value: string) {
    setValue(`questions.${questionIndex}.value`, value, { shouldDirty: true, shouldTouch: true, shouldValidate: true });
  }

  return (
    <div className='mt-3'>
      <div className='flex items-center space-x-2'>
        <Switch
          id='has-displayLogic'
          onCheckedChange={handleHasDisplayLogicChanged}
          checked={hasDisplayLogic}
          disabled={availableParentQuestions.length === 0}
        />
        <Label htmlFor='has-displayLogic' className='flex items-center gap-2'>
          {t('questionEditor.question.displayLogic')}
          {availableParentQuestions.length === 0 && (
            <TooltipProvider>
              <Tooltip delayDuration={300}>
                <TooltipTrigger type='button'>
                  <InformationCircleIcon width={18} />
                </TooltipTrigger>
                <TooltipContent>
                  <p className='text-slate-700 text-sm w-[250px]'>{t('questionEditor.question.displayLogic.invalid')}</p>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          )}
        </Label>
      </div>

      <Collapsible open={hasDisplayLogic}>
        <CollapsibleContent className='justify-left flex flex-col mt-3'>
          <div className='justify-left flex flex-col mt-3'>
            <span className='mb-2 text-sm'>{t('questionEditor.question.displayLogic.chooseQuestion')}:</span>
            <Select name='question' onValueChange={handleParentQuestionSelected} value={parentQuestion?.questionId}>
              <SelectTrigger className='min-w-fit flex-1'>
                <SelectValue placeholder='Select question' className='text-xs lg:text-sm' />
              </SelectTrigger>
              <SelectContent side='top'>
                {availableParentQuestions.map((question) => (
                  <SelectItem key={question.questionId} value={question.questionId} >
                    {question.text[languageCode]}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div className='justify-left flex flex-col mt-3'>
            {parentQuestion && (
              <Select value={question?.condition} onValueChange={handleConditionChanged}>
                <SelectTrigger className='min-w-fit flex-1'>
                  <SelectValue placeholder='Select condition' className='text-xs lg:text-sm' />
                </SelectTrigger>
                <SelectContent>
                  {conditions[parentQuestion.$questionType.toString()]!.map((condition) => (
                    <SelectItem key={condition} value={condition} title={condition} className='text-xs lg:text-sm'>
                      {condition}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          </div>
          {parentQuestion?.$questionType === QuestionType.NumberQuestionType && (
            <div className='justify-left flex flex-col mt-3'>
              <Input
                type='number'
                value={question?.value}
                onChange={(e) => {
                  handleValueChanged(e.target.value);
                }}
                placeholder='value'
              />
            </div>
          )}
          {parentQuestion?.$questionType === QuestionType.RatingQuestionType && (
            <div className='justify-left flex flex-col mt-3'>
              <Select value={question?.value} onValueChange={handleValueChanged}>
                <SelectTrigger className='min-w-fit flex-1'>
                  <SelectValue placeholder='Select rating' className='text-xs lg:text-sm' />
                </SelectTrigger>
                <SelectContent>
                  {[...Array(ratingScaleToNumber(parentQuestion!.scale)).keys()]
                    .map((value) => value + 1)
                    .map((value) => (
                      <SelectItem
                        key={value}
                        value={value.toString()}
                        title={value.toString()}
                        className='text-xs lg:text-sm'>
                        {value}
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
            </div>
          )}
          {(parentQuestion?.$questionType === QuestionType.MultiSelectQuestionType ||
            parentQuestion?.$questionType === QuestionType.SingleSelectQuestionType) && (
              <div className='justify-left flex flex-col mt-3'>
                <Select value={question?.value} onValueChange={handleValueChanged}>
                  <SelectTrigger className='min-w-fit flex-1'>
                    <SelectValue placeholder='Select option' className='text-xs lg:text-sm' />
                  </SelectTrigger>
                  <SelectContent>
                    {parentQuestion!.options.map((option) => (
                      <SelectItem
                        key={option.optionId}
                        value={option.optionId}
                        title={option.text[languageCode]}
                        className='text-xs lg:text-sm'>
                        {option.text[languageCode]}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            )}
        </CollapsibleContent>
      </Collapsible>
    </div>
  );
}
