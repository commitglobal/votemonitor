import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useTranslation } from 'react-i18next';

import { ZFormType } from '@/common/types';
import LanguageSelect from '@/containers/LanguageSelect';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { mapFormType } from '@/lib/utils';
import { useFormContext } from 'react-hook-form';
import { EditFormType } from '../EditForm/EditForm';

export interface EditFormTranslationDetailsProps {
  languageCode: string;
}

function EditFormTranslationDetails({ languageCode }: EditFormTranslationDetailsProps) {
  const { t } = useTranslation();
  const form = useFormContext<EditFormType>();
  const isMonitoringNgoForCitizenReporting = useCurrentElectionRoundStore(s => s.isMonitoringNgoForCitizenReporting);

  return (
    <div className='md:inline-flex md:space-x-6'>
      <div className='space-y-4 md:w-1/2'>
        <FormField
          control={form.control}
          name="formType"
          render={({ field }) => (
            <FormItem>
              <FormLabel>{t('form.field.formType')}</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value} disabled>
                <FormControl>
                  <SelectTrigger className='truncate'>
                    <SelectValue placeholder="Select a form type" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value={ZFormType.Values.Opening}>{mapFormType(ZFormType.Values.Opening)}</SelectItem>
                  <SelectItem value={ZFormType.Values.Voting}>{mapFormType(ZFormType.Values.Voting)}</SelectItem>
                  <SelectItem value={ZFormType.Values.ClosingAndCounting}>{mapFormType(ZFormType.Values.ClosingAndCounting)}</SelectItem>
                  {isMonitoringNgoForCitizenReporting && <SelectItem value={ZFormType.Values.CitizenReporting}>{mapFormType(ZFormType.Values.CitizenReporting)}</SelectItem>}
                  <SelectItem value={ZFormType.Values.IncidentReporting}>{mapFormType(ZFormType.Values.IncidentReporting)}</SelectItem>
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
              <FormLabel>{t('form.field.languageCode')}</FormLabel>
              <FormControl>
                <LanguageSelect
                  languageCode={field.value}
                  disabled
                />

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

export default EditFormTranslationDetails