import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useTranslation } from 'react-i18next';

import { changeLanguageCode, QuestionType } from '@/common/types';
import LanguageSelect from '@/containers/LanguageSelect';
import { InformationCircleIcon } from '@heroicons/react/24/outline';
import { useFormContext } from 'react-hook-form';
import { FormType, mapFormType } from '../../models/form';
import { EditFormType } from './EditForm';

export interface EditFormDetailsProps {
  languageCode: string;
}

function EditFormDetails({ languageCode }: EditFormDetailsProps) {
  const { t } = useTranslation();
  const form = useFormContext<EditFormType>();

  const handleLanguageChange = (newLanguageCode: string): void => {
    const formValues = form.getValues();
    form.setValue('name', changeLanguageCode(formValues.name, languageCode, newLanguageCode));

    form.setValue('description', changeLanguageCode(formValues.description, languageCode, newLanguageCode));

    form.setValue('languageCode', newLanguageCode);
    form.setValue('languages', [...formValues.languages.filter(l => l !== languageCode), newLanguageCode]);

    formValues.questions.forEach((question, index) => {
      if (question.$questionType === QuestionType.NumberQuestionType || question.$questionType === QuestionType.TextQuestionType) {
        form.setValue(`questions.${index}`, {
          ...question,
          languageCode: newLanguageCode,
          text: changeLanguageCode(question.text, languageCode, newLanguageCode),
          helptext: changeLanguageCode(question.helptext, languageCode, newLanguageCode),
          inputPlaceholder: changeLanguageCode(question.inputPlaceholder, languageCode, newLanguageCode),
        });
      }

      if (question.$questionType === QuestionType.DateQuestionType || question.$questionType === QuestionType.RatingQuestionType) {
        form.setValue(`questions.${index}`, {
          ...question,
          languageCode: newLanguageCode,
          text: changeLanguageCode(question.text, languageCode, newLanguageCode)!,
          helptext: changeLanguageCode(question.helptext, languageCode, newLanguageCode),
        });
      }

      if (question.$questionType === QuestionType.SingleSelectQuestionType || question.$questionType === QuestionType.MultiSelectQuestionType) {
        form.setValue(`questions.${index}`, {
          ...question,
          languageCode: newLanguageCode,
          text: changeLanguageCode(question.text, languageCode, newLanguageCode)!,
          helptext: changeLanguageCode(question.helptext, languageCode, newLanguageCode),
          options: question.options?.map(option => ({
            ...option,
            text: changeLanguageCode(option.text, languageCode, newLanguageCode)!,
          }))
        });
      }

      form.trigger(`questions.${index}`);
    });

    form.trigger('questions');
  };

  return (
    <div className='md:inline-flex md:space-x-6'>
      <div className='md:w-1/2 space-y-4'>
        <FormField
          control={form.control}
          name="formType"
          render={({ field }) => (
            <FormItem>
              <FormLabel>{t('form.field.formType')}</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger className='truncate'>
                    <SelectValue placeholder="Select a form type" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value={FormType.Opening}>{mapFormType(FormType.Opening)}</SelectItem>
                  <SelectItem value={FormType.Voting}>{mapFormType(FormType.Voting)}</SelectItem>
                  <SelectItem value={FormType.ClosingAndCounting}>{mapFormType(FormType.ClosingAndCounting)}</SelectItem>
                  <SelectItem value={FormType.Other}>{mapFormType(FormType.Other)}</SelectItem>
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />
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
                <Input placeholder={t('form.placeholder.name')}
                  {...field}
                  {...fieldState}
                  value={field.value[languageCode]}
                  onChange={event => field.onChange({
                    ...field.value,
                    [languageCode]: event.target.value
                  })} />
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
          <div><InformationCircleIcon width={24} height={24} /></div>
          <div className='text-sm ml-2'>Base language is the language a form is initially written in. You can multiple translations after you finalize the form in base language.</div>
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
                  onChange={event => field.onChange({
                    ...field.value,
                    [languageCode]: event.target.value
                  })} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </div>
  )


}

export default EditFormDetails