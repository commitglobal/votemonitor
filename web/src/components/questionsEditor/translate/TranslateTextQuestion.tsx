import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { EditFormType } from '@/components/FormEditor/FormEditor';
import { useFormContext, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';

export interface TranslateTextQuestionProps {
  questionIndex: number;
}

function TranslateTextQuestion({ questionIndex }: TranslateTextQuestionProps) {
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
                  onChange={event => field.onChange({
                    ...field.value,
                    [languageCode]: event.target.value
                  })}
                  placeholder={field.value[defaultLanguageCode]}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          );
        }}
      />
    </>
  );
}
export default TranslateTextQuestion;
