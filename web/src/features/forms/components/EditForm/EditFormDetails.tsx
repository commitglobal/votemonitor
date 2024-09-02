import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useTranslation } from 'react-i18next';

import { QuestionType, ZFormType } from '@/common/types';
import LanguageSelect from '@/containers/LanguageSelect';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { InformationCircleIcon } from '@heroicons/react/24/outline';
import { useFormContext } from 'react-hook-form';
import { EditFormType } from './EditForm';
import { changeLanguageCode, mapFormType } from '@/lib/utils';

export interface EditFormDetailsProps {
  languageCode: string;
}

function EditFormDetails({ languageCode }: EditFormDetailsProps) {
  const { t } = useTranslation();
  const form = useFormContext<EditFormType>();
  const isMonitoringNgoForCitizenReporting = useCurrentElectionRoundStore(s => s.isMonitoringNgoForCitizenReporting);

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
                  <SelectItem value={ZFormType.Values.Opening}>{mapFormType(ZFormType.Values.Opening)}</SelectItem>
                  <SelectItem value={ZFormType.Values.Voting}>{mapFormType(ZFormType.Values.Voting)}</SelectItem>
                  <SelectItem value={ZFormType.Values.ClosingAndCounting}>{mapFormType(ZFormType.Values.ClosingAndCounting)}</SelectItem>
                  {isMonitoringNgoForCitizenReporting && <SelectItem value={ZFormType.Values.CitizenReport}>{mapFormType(ZFormType.Values.CitizenReport)}</SelectItem>}
                  <SelectItem value={ZFormType.Values.Other}>{mapFormType(ZFormType.Values.Other)}</SelectItem>
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