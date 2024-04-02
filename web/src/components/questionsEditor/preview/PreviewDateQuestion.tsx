import { AnswerType, BaseAnswer, DateAnswer, DateAnswerSchema, DateQuestion } from '@/common/types'
import { zodResolver } from '@hookform/resolvers/zod';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { useForm } from 'react-hook-form';
import { Popover, PopoverContent, PopoverTrigger } from '../../ui/popover';
import { cn } from '@/lib/utils';
import { Calendar } from '../../ui/calendar';
import { format, parseISO, formatISO } from 'date-fns';
import { CalendarIcon } from 'lucide-react';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';

export interface PreviewDateQuestionProps {
  languageCode: string;
  question: DateQuestion;
  answer: DateAnswer;
  isFirstQuestion: boolean;
  isLastQuestion: boolean;
  onSubmitAnswer: (answer: BaseAnswer) => void;
  onBackButtonClicked: () => void;
}

function PreviewDateQuestion({
  languageCode,
  question,
  answer,
  isFirstQuestion,
  isLastQuestion,
  onSubmitAnswer,
  onBackButtonClicked }: PreviewDateQuestionProps) {
  const { t } = useTranslation();

  const form = useForm<DateAnswer>({
    resolver: zodResolver(DateAnswerSchema),
    defaultValues: answer ?? {
      questionId: question.id,
      $answerType: AnswerType.DateAnswerType,
    }
  });

  return (<Form {...form}>
    <form onSubmit={form.handleSubmit(onSubmitAnswer)}>
      <div className='grid gap-6 py-4 sm:grid-cols-2'>
        <FormField
          control={form.control}
          name='date'
          render={({ field }) => (
            <FormItem className='flex flex-col'>
              <FormLabel>{question.text[languageCode]}</FormLabel>
              <Popover>
                <PopoverTrigger asChild>
                  <FormControl>
                    <Button
                      variant={'outline'}
                      className={cn('w-full pl-3 text-left font-normal', !field.value && 'text-muted-foreground')}>
                      {field.value ? format(field.value, 'PPP') : <span>Pick a date</span>}
                      <CalendarIcon className='w-4 h-4 ml-auto opacity-50' />
                    </Button>
                  </FormControl>
                </PopoverTrigger>
                <PopoverContent className='w-auto p-0' align='start'>
                  <Calendar
                    mode='single'
                    selected={field.value ? parseISO(field.value) : undefined}
                    onSelect={(day) => {
                      form.setValue("date", formatISO(day!, { representation: 'date' }))
                    }}
                    disabled={(date) => date < new Date()}
                    initialFocus
                  />
                </PopoverContent>
              </Popover>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
      <div className="mt-4 flex w-full justify-between">
        {!isFirstQuestion && (
          <Button
            type='button'
            onClick={() => {
              onBackButtonClicked();
            }}
          >
            {t('navigation.button.back')}
          </Button>
        )}
        <div></div>
        <Button type='submit'>
          {isLastQuestion ? t('navigation.button.submit') : t('navigation.button.next')}
        </Button>
      </div>
    </form>
  </Form>)
}


export default PreviewDateQuestion
