import { useTranslation } from 'react-i18next';

import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { EditFormType } from '@/features/forms/components/EditForm/EditForm';
import { useFormContext, useWatch } from 'react-hook-form';
import { Input } from '../../ui/input';

export interface TranslateNumberQuestionProps {
  questionIndex: number;
}

function TranslateNumberQuestion({ questionIndex }: TranslateNumberQuestionProps) {
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

  return (
    <>
      <FormField
        control={control}
        name={`questions.${questionIndex}.inputPlaceholder` as const}
        render={({ field, fieldState }) => {
          return (
            <FormItem>
              <FormLabel>{t('questionEditor.question.inputPlaceholder')}</FormLabel>
              <FormControl>
                <Input
                  {...field}
                  {...fieldState}
                  value={field.value[languageCode]}
                  placeholder={field.value[defaultLanguageCode]}
                  onChange={event => field.onChange({
                    ...field.value,
                    [languageCode]: event.target.value
                  })} />
              </FormControl>
              <FormMessage />
            </FormItem>
          );
        }}
      />
    </>
  );
}

export default TranslateNumberQuestion;
