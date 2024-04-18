import { AnswerType, BaseAnswer, DateAnswer, DateAnswerSchema, DateQuestion } from '@/common/types'
import { zodResolver } from '@hookform/resolvers/zod';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { useForm } from 'react-hook-form';
import { Popover, PopoverContent, PopoverTrigger } from '../../ui/popover';
import { cn } from '@/lib/utils';
import { Calendar } from '../../ui/calendar';
import { format, parseISO, formatISO } from 'date-fns';
import { CalendarIcon } from 'lucide-react';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';
import { Description, Field, Label } from '@/components/ui/fieldset';
import { useState } from 'react';

export interface PreviewDateQuestionProps {
  languageCode: string;
  question: DateQuestion;
  answer: DateAnswer;
  isFirstQuestion?: boolean;
  isLastQuestion?: boolean;
  onSubmitAnswer?: (answer: BaseAnswer) => void;
  onBackButtonClicked?: () => void;
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
  const [date, setDate] = useState<string>('');
  const form = useForm<DateAnswer>({
    resolver: zodResolver(DateAnswerSchema),
    defaultValues: answer ?? {
      questionId: question.id,
      $answerType: AnswerType.DateAnswerType,
    }
  });

  return (
    <Field>
      <Label>{question.code} - {question.text[languageCode]}</Label>
      {!!question.helptext && <Description>{question.helptext[languageCode]}</Description>}
      <Popover>
        <PopoverTrigger asChild>
          <Button
            variant={'outline'}
            className={cn('w-full pl-3 text-left font-normal', !date && 'text-muted-foreground')}>
            {date ? format(date, 'PPP') : <span>Pick a date</span>}
            <CalendarIcon className='w-4 h-4 ml-auto opacity-50' />
          </Button>
        </PopoverTrigger>
        <PopoverContent className='w-auto p-0' align='start'>
          <Calendar
            mode='single'
            selected={date ? parseISO(date) : undefined}
            onSelect={(day) => {
              setDate(formatISO(day!, { representation: 'date' }))
            }}
            disabled={(date) => date < new Date()}
            initialFocus
          />
        </PopoverContent>
      </Popover>
    </Field>)
}


export default PreviewDateQuestion
