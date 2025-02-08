import {
  QuestionType
} from '@/common/types';
import { FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { EditFormType } from '@/components/FormEditor/FormEditor';
import { cn } from '@/lib/utils';
import { FlagIcon } from '@heroicons/react/24/solid';
import { CheckCircle, CheckSquare, PencilLine } from 'lucide-react';
import { useFormContext, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';

export interface TranslateMultiSelectQuestionProps {
  questionIndex: number;
}

function TranslateSelectQuestion({ questionIndex }: TranslateMultiSelectQuestionProps) {
  const { t } = useTranslation();
  const { control } = useFormContext<EditFormType>();

  const languageCode = useWatch({
    control,
    name: `languageCode`,
  });

  const defaultLanguageCode = useWatch({
    control,
    name: `defaultLanguage`,
  });

  const questionType = useWatch({
    control,
    name: `questions.${questionIndex}.$questionType`
  });

  const options = useWatch({
    control,
    name: `questions.${questionIndex}.options`,
    defaultValue: []
  });


  return (
    <div>
      <Label htmlFor='choices'>Options</Label>
      <div className='space-y-4' id='choices'>
        {options &&
          options.map((option, optionIndex) => (
            <div key={optionIndex} className='inline-flex w-full items-center space-y-4 '>
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
                            placeholder={field.value[defaultLanguageCode]}
                          />
                          <div className='inline-flex items-center space-x-2 ml-2 w-[100px] '>
                            <FlagIcon
                              className={cn('h-4 w-4 cursor-pointer', {
                                'text-slate-700 hover:text-red-600': !option.isFlagged,
                                'text-red-600 hover:text-slate-00': option.isFlagged
                              })} />

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
      </div>
    </div>
  );
}

export default TranslateSelectQuestion;
