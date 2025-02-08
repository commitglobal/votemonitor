import { RatingScaleType } from '@/common/types';
import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { EditFormType } from '@/components/FormEditor/FormEditor';
import { useMemo } from 'react';
import { useFormContext, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';


export interface EditRatingQuestionProps {
  questionIndex: number;
}

function EditRatingQuestion({ questionIndex }: EditRatingQuestionProps) {
  const { t } = useTranslation();
  const { control } = useFormContext<EditFormType>();

  const languageCode = useWatch({
    control,
    name: `languageCode`,
  });

  const ratingScales = useMemo(() => [
    { label: t('questionEditor.question.ratingScale.oneTo3'), value: RatingScaleType.OneTo3 },
    { label: t('questionEditor.question.ratingScale.oneTo4'), value: RatingScaleType.OneTo4 },
    { label: t('questionEditor.question.ratingScale.oneTo5'), value: RatingScaleType.OneTo5 },
    { label: t('questionEditor.question.ratingScale.oneTo6'), value: RatingScaleType.OneTo6 },
    { label: t('questionEditor.question.ratingScale.oneTo7'), value: RatingScaleType.OneTo7 },
    { label: t('questionEditor.question.ratingScale.oneTo8'), value: RatingScaleType.OneTo8 },
    { label: t('questionEditor.question.ratingScale.oneTo9'), value: RatingScaleType.OneTo9 },
    { label: t('questionEditor.question.ratingScale.oneTo10'), value: RatingScaleType.OneTo10 },
  ], []);


  return (
    <div className='space-y-4'>
      <FormField
        control={control}
        name={`questions.${questionIndex}.scale` as const}
        render={({ field }) => (
          <FormItem>
            <FormLabel>{t('questionEditor.ratingQuestion.scale')}</FormLabel>
            <Select onValueChange={field.onChange} defaultValue={field.value}>
              <FormControl>
                <SelectTrigger>
                  <SelectValue placeholder="Select a rating scale" />
                </SelectTrigger>
              </FormControl>
              <SelectContent>
                {ratingScales.map(scale => (<SelectItem value={scale.value} key={scale.value}>{scale.label}</SelectItem>))}
              </SelectContent>
            </Select>
            <FormMessage />
          </FormItem>
        )}
      />

      <FormField
        control={control}
        name={`questions.${questionIndex}.lowerLabel` as const}
        render={({ field }) => (
          <FormItem>
            <FormLabel>{t('questionEditor.question.scale.lowerLabel')}</FormLabel>
            <FormControl>
              <Input {...field}
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
        control={control}
        name={`questions.${questionIndex}.upperLabel` as const}
        render={({ field }) => (
          <FormItem>
            <FormLabel>{t('questionEditor.question.scale.upperLabel')}</FormLabel>
            <FormControl>
              <Input
                {...field}
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
    </div >
  )
}

export default EditRatingQuestion
