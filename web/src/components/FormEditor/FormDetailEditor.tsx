import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useTranslation } from 'react-i18next';

import { QuestionType, FormType } from '@/common/types';
import LanguageSelect from '@/containers/LanguageSelect';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { InformationCircleIcon } from '@heroicons/react/24/outline';
import { useFormContext, useWatch } from 'react-hook-form';
import { EditFormType } from './FormEditor';
import { changeLanguageCode, mapFormType } from '@/lib/utils';
import { useEffect, useRef, useState } from 'react';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';

export interface FormDetailEditorProps {
  languageCode: string;
  hasCitizenReportingOption: boolean;
}

function FormDetailEditor({ languageCode ,hasCitizenReportingOption}: FormDetailEditorProps) {
  const { t } = useTranslation();
  const form = useFormContext<EditFormType>();
  const formType = useWatch({ control: form.control, name: 'formType' });
  const icon = useWatch({ control: form.control, name: 'icon' });

  const [svgDimensions, setSvgDimensions] = useState({ width: 0, height: 0 });
  const [error, setError] = useState('');
  const previewRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (icon && previewRef.current) {
      try {
        // Clear previous content and error
        previewRef.current.innerHTML = '';
        setError('');

        // Create a temporary div to hold the SVG
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = icon.trim();

        // Check if the input is a valid SVG
        const svgElement = tempDiv.querySelector('svg');
        if (!svgElement) {
          throw new Error('Invalid SVG input');
        }

        // Append the SVG to the preview div
        previewRef.current.appendChild(svgElement);

        // Get the rendered dimensions
        const bbox = svgElement.getBBox();
        setSvgDimensions({
          width: Math.round(bbox.width),
          height: Math.round(bbox.height),
        });
      } catch (err) {
        setError('Invalid SVG input');
        setSvgDimensions({ width: 0, height: 0 });
      }
    } else {
      setSvgDimensions({ width: 0, height: 0 });
    }
  }, [icon]);

  const handleLanguageChange = (newLanguageCode: string): void => {
    const formValues = form.getValues();
    form.setValue('name', changeLanguageCode(formValues.name, languageCode, newLanguageCode));

    form.setValue('description', changeLanguageCode(formValues.description, languageCode, newLanguageCode));

    form.setValue('languageCode', newLanguageCode);
    form.setValue('languages', [...formValues.languages.filter((l) => l !== languageCode), newLanguageCode]);

    formValues.questions.forEach((question, index) => {
      if (
        question.$questionType === QuestionType.NumberQuestionType ||
        question.$questionType === QuestionType.TextQuestionType
      ) {
        form.setValue(`questions.${index}`, {
          ...question,
          languageCode: newLanguageCode,
          text: changeLanguageCode(question.text, languageCode, newLanguageCode),
          helptext: changeLanguageCode(question.helptext, languageCode, newLanguageCode),
          inputPlaceholder: changeLanguageCode(question.inputPlaceholder, languageCode, newLanguageCode),
        });
      }

      if (
        question.$questionType === QuestionType.DateQuestionType ||
        question.$questionType === QuestionType.RatingQuestionType
      ) {
        form.setValue(`questions.${index}`, {
          ...question,
          languageCode: newLanguageCode,
          text: changeLanguageCode(question.text, languageCode, newLanguageCode)!,
          helptext: changeLanguageCode(question.helptext, languageCode, newLanguageCode),
        });
      }

      if (
        question.$questionType === QuestionType.SingleSelectQuestionType ||
        question.$questionType === QuestionType.MultiSelectQuestionType
      ) {
        form.setValue(`questions.${index}`, {
          ...question,
          languageCode: newLanguageCode,
          text: changeLanguageCode(question.text, languageCode, newLanguageCode)!,
          helptext: changeLanguageCode(question.helptext, languageCode, newLanguageCode),
          options: question.options?.map((option) => ({
            ...option,
            text: changeLanguageCode(option.text, languageCode, newLanguageCode)!,
          })),
        });
      }

      form.trigger(`questions.${index}`);
    });

    form.trigger('questions');
  };

  return (
    <div className='md:inline-flex md:space-x-6'>
      <div className='space-y-4 md:w-1/2'>
        <FormField
          control={form.control}
          name='formType'
          render={({ field }) => (
            <FormItem>
              <FormLabel>{t('form.field.formType')}</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger className='truncate'>
                    <SelectValue placeholder='Select a form type' />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value={FormType.Opening}>{mapFormType(FormType.Opening)}</SelectItem>
                  <SelectItem value={FormType.Voting}>{mapFormType(FormType.Voting)}</SelectItem>
                  <SelectItem value={FormType.ClosingAndCounting}>
                    {mapFormType(FormType.ClosingAndCounting)}
                  </SelectItem>
                  {hasCitizenReportingOption && (
                    <SelectItem value={FormType.CitizenReporting}>
                      {mapFormType(FormType.CitizenReporting)}
                    </SelectItem>
                  )}
                  <SelectItem value={FormType.IncidentReporting}>
                    {mapFormType(FormType.IncidentReporting)}
                  </SelectItem>
                  <SelectItem value={FormType.Other}>{mapFormType(FormType.Other)}</SelectItem>
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        {formType === FormType.CitizenReporting && hasCitizenReportingOption ? (
          <>
            <FormField
              control={form.control}
              name='icon'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>{t('form.field.icon')}</FormLabel>
                  <FormControl>
                    <Textarea placeholder={t('form.placeholder.icon')} {...field} {...fieldState} rows={5} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className='space-y-2'>
              {error ? <p className='text-red-500'>{error}</p> : <div ref={previewRef} />}
              {svgDimensions.width > 0 && svgDimensions.height > 0 && (
                <p className='text-sm text-muted-foreground'>
                  {svgDimensions.width}px x {svgDimensions.height}px
                </p>
              )}
            </div>
          </>
        ) : null}

        <FormField
          control={form.control}
          name='code'
          render={({ field, fieldState }) => (
            <FormItem>
              <FormLabel>{t('form.field.code')}</FormLabel>
              <FormControl>
                <Input placeholder={t('form.placeholder.code')} {...field} {...fieldState} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name='name'
          render={({ field, fieldState }) => (
            <FormItem>
              <FormLabel>{t('form.field.name')}</FormLabel>
              <FormControl>
                <Input
                  placeholder={t('form.placeholder.name')}
                  {...field}
                  {...fieldState}
                  value={field.value[languageCode]}
                  onChange={(event) =>
                    field.onChange({
                      ...field.value,
                      [languageCode]: event.target.value,
                    })
                  }
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name='languageCode'
          render={({ field }) => (
            <FormItem className='flex flex-col'>
              <FormLabel>{t('form.field.defaultLanguage')}</FormLabel>
              <FormControl>
                <LanguageSelect
                  languageCode={field.value}
                  onLanguageSelected={(value) => {
                    handleLanguageChange(value);
                  }}
                />
              </FormControl>
            </FormItem>
          )}
        />

        <div className='inline-flex text-slate-700'>
          <div>
            <InformationCircleIcon width={24} height={24} />
          </div>
          <div className='ml-2 text-sm'>
            Base language is the language a form is initially written in. You can multiple translations after you
            finalize the form in base language.
          </div>
        </div>
      </div>
      <div className='md:w-1/2'>
        <FormField
          control={form.control}
          name='description'
          render={({ field, fieldState }) => (
            <FormItem>
              <FormLabel>{t('form.field.description')}</FormLabel>
              <FormControl>
                <Textarea
                  rows={10}
                  cols={100}
                  {...field}
                  {...fieldState}
                  placeholder={t('form.placeholder.description')}
                  value={field.value ? field.value[languageCode] : ''}
                  onChange={(event) =>
                    field.onChange({
                      ...field.value,
                      [languageCode]: event.target.value,
                    })
                  }
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </div>
  );
}

export default FormDetailEditor;
