import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useTranslation } from 'react-i18next';

import { FormType } from '@/common/types';
import LanguageSelect from '@/containers/LanguageSelect';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { mapFormType } from '@/lib/utils';
import { useFormContext } from 'react-hook-form';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { EditFormType } from '../FormEditor/FormEditor';
import { FormFull } from '@/features/forms/models';
import { FormTemplateFull } from '@/features/form-templates/models';

export interface FormDetailsTranslationEditorProps {
  languageCode: string;
  hasCitizenReportingOption: boolean;
}

function FormDetailsTranslationEditor({ languageCode, hasCitizenReportingOption }: FormDetailsTranslationEditorProps) {
  const { t } = useTranslation();
  const form = useFormContext<EditFormType>();

  return (
    <div className='md:inline-flex md:space-x-6'>
      <div className='space-y-4 md:w-1/2'>
        <FormField
          control={form.control}
          name='formType'
          render={({ field }) => (
            <FormItem>
              <FormLabel>{t('form.field.formType')}</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value} disabled>
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
                    <SelectItem value={FormType.CitizenReporting}>{mapFormType(FormType.CitizenReporting)}</SelectItem>
                  )}
                  <SelectItem value={FormType.IncidentReporting}>{mapFormType(FormType.IncidentReporting)}</SelectItem>
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
                <Input placeholder={t('form.placeholder.code')} {...field} {...fieldState} disabled />
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
              <FormLabel>{t('form.field.languageCode')}</FormLabel>
              <FormControl>
                <LanguageSelect languageCode={field.value} disabled />
              </FormControl>
            </FormItem>
          )}
        />
      </div>
      <div className='md:w-1/2'>
        <FormField
          control={form.control}
          name='description'
          render={({ field }) => (
            <FormItem>
              <FormLabel>{t('form.field.description')}</FormLabel>
              <FormControl>
                <Textarea
                  rows={10}
                  cols={100}
                  {...field}
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

export default FormDetailsTranslationEditor;
