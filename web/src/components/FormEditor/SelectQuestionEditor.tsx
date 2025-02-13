import {
  QuestionType
} from '@/common/types';
import { Button } from '@/components/ui/button';
import { FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { EditFormType } from '@/components/FormEditor/FormEditor';
import { cn, newTranslatedString } from '@/lib/utils';
import { FlagIcon, PlusIcon, TrashIcon } from '@heroicons/react/24/solid';
import { CheckCircle, CheckSquare, PencilLine } from 'lucide-react';
import { useEffect } from 'react';
import { useFormContext, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { v4 as uuidv4 } from 'uuid';

export interface SelectQuestionEditorProps {
  questionIndex: number;
}

function SelectQuestionEditor({ questionIndex }: SelectQuestionEditorProps) {
  const { t } = useTranslation();
  const { control, setValue, trigger } = useFormContext<EditFormType>();

  const questionType = useWatch({
    control,
    name: `questions.${questionIndex}.$questionType`
  });

  const options = useWatch({
    control,
    name: `questions.${questionIndex}.options`,
    defaultValue: []
  });

  const languageCode = useWatch({
    control,
    name: `languageCode`,
  });

  const availableLanguages = useWatch({
    control,
    name: `languages`,
  });

  function addOption(optionIdx?: number) {
    let newOptions = [...options];
    const freeTextOption = newOptions.find((option) => option.isFreeText);
    if (freeTextOption) {
      newOptions = newOptions.filter((option) => !option.isFreeText);
    }

    const newOption = {
      optionId: uuidv4(),
      text: newTranslatedString(availableLanguages, languageCode, ''),
      isFlagged: false,
      isFreeText: false,
    };
    if (optionIdx !== undefined) {
      newOptions.splice(optionIdx + 1, 0, newOption);
    } else {
      newOptions.push(newOption);
    }
    if (freeTextOption) {
      newOptions.push(freeTextOption);
    }

    setValue(`questions.${questionIndex}.options`, newOptions, {
      shouldValidate: true,
      shouldDirty: true,
      shouldTouch: true
    });

    trigger(`questions.${questionIndex}.options`);
  }

  function changeQuestionType(newQuestionType: QuestionType) {
    setValue(`questions.${questionIndex}.$questionType`, newQuestionType, {
      shouldValidate: true,
      shouldDirty: true
    });
  }

  function addFreeTextOption() {
    if (options.filter((option) => option.isFreeText).length > 0) return;
    const newOptions = [...options];
    const freeTextOption = {
      optionId: uuidv4(),
      text: newTranslatedString(availableLanguages, languageCode, ''),
      isFlagged: false,
      isFreeText: true,
    };
    newOptions.push(freeTextOption);

    setValue(`questions.${questionIndex}.options`, newOptions, {
      shouldValidate: true,
      shouldDirty: true,
      shouldTouch: true
    });
    trigger(`questions.${questionIndex}.options`);
  }

  function deleteOption(optionId: string) {
    const newOptions = options.filter((option) => option.optionId !== optionId);

    setValue(`questions.${questionIndex}.options`, newOptions, {
      shouldValidate: true,
      shouldDirty: true,
      shouldTouch: true
    });

    trigger(`questions.${questionIndex}.options`);
  }

  useEffect(() => {
    trigger(`questions.${questionIndex}.options`);
  }, [options, questionIndex, trigger]);

  return (
    <div>
      <Label htmlFor='choices'>Options</Label>
      <div className='space-y-4' id='choices'>
        {options &&
          options.map((option, optionIndex) => (
            <div key={option.optionId} className='inline-flex w-full items-center space-y-4 '>
              <div className='inline-flex justify-between w-full'>
                <FormField
                  control={control}
                  name={`questions.${questionIndex}.options.${optionIndex}.text` as const}
                  render={({ field, fieldState }) => (
                    <FormItem className='w-full'>
                      <FormControl>
                        <div className='inline-flex justify-between w-full'>
                          <div className='mt-2 mr-2 h-4 w-4'>
                            {option.isFreeText ? (
                              <PencilLine className='h-full w-full text-slate-700' />
                            ) : questionType === QuestionType.SingleSelectQuestionType ? (
                              <CheckCircle className='h-full w-full text-slate-700' />
                            ) : (
                              <CheckSquare className='h-full w-full text-slate-700' />
                            )}
                          </div>
                          <Input {...field} {...fieldState}
                            value={field.value[languageCode]}
                            onChange={event => field.onChange({
                              ...field.value,
                              [languageCode]: event.target.value
                            })}
                          />
                          <div className='inline-flex items-center space-x-2 ml-2 w-[100px] '>
                            <FlagIcon
                              className={cn('h-4 w-4 cursor-pointer', {
                                'text-slate-700 hover:text-red-600': !option.isFlagged,
                                'text-red-600 hover:text-slate-00': option.isFlagged
                              })}
                              onClick={() =>
                                setValue(`questions.${questionIndex}.options.${optionIndex}.isFlagged`, !option.isFlagged, {
                                  shouldValidate: true,
                                  shouldDirty: true
                                })
                              }
                            />

                            {options && options.length > 2 && (
                              <TrashIcon
                                className={'h-4 w-4 cursor-pointer text-slate-400 hover:text-slate-500'}
                                onClick={() => deleteOption(option.optionId)}
                              />
                            )}

                            {!option.isFreeText && (
                              <PlusIcon
                                className={'h-4 w-4 cursor-pointer text-slate-400 hover:text-slate-500'}
                                onClick={() => addOption(optionIndex)}
                              />
                            )}
                          </div>
                        </div>
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
            </div>
          ))}
        <div className='flex items-center justify-between space-y-2'>
          {options.filter((c) => c.isFreeText).length === 0 && (
            <Button
              size='sm'
              variant='outline'
              type='button'
              onClick={() => addFreeTextOption()}>
              {t('questionEditor.selectQuestion.addFreeTextOption')}
            </Button>
          )}
          <Button
            size='sm'
            variant='outline'
            type='button'
            onClick={() => {
              questionType === QuestionType.SingleSelectQuestionType
                ? changeQuestionType(QuestionType.MultiSelectQuestionType)
                : changeQuestionType(QuestionType.SingleSelectQuestionType);
            }}>
            {questionType === QuestionType.SingleSelectQuestionType
              ? t('questionEditor.selectQuestion.toMultiSelectQuestion')
              : t('questionEditor.selectQuestion.toSingleSelectQuestion')}
          </Button>
        </div>
      </div>
    </div>
  );
}

export default SelectQuestionEditor;
