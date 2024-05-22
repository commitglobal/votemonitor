import { DateAnswer, DateQuestion } from '@/common/types';
import { Popover, PopoverContent, PopoverTrigger } from '../../ui/popover';
import { cn } from '@/lib/utils';
import { Calendar } from '../../ui/calendar';
import { format, parseISO, formatISO } from 'date-fns';
import { CalendarIcon } from 'lucide-react';
import { Button } from '../../ui/button';
import { Description, Field, Label } from '@/components/ui/fieldset';
import { useState } from 'react';
import { useParams } from '@tanstack/react-router';

export interface PreviewDateQuestionProps {
  languageCode: string;
  question: DateQuestion;
  answer: DateAnswer;
  setAnswer: (answer: DateAnswer) => void;
}

function PreviewDateQuestion({ languageCode, question, answer, setAnswer }: PreviewDateQuestionProps) {
  const [date, setDate] = useState<string>('');

  const params: any = useParams({
    strict: false,
  });

  return (
    <Field>
      <Label>
        {question.code} - {question.text[params['languageCode'] ? params['languageCode'] : languageCode]}
      </Label>
      {!!question.helptext && (
        <Description>{question.helptext[params['languageCode'] ? params['languageCode'] : languageCode]}</Description>
      )}
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
              setDate(formatISO(day!, { representation: 'date' }));
            }}
            disabled={(date) => date < new Date()}
            initialFocus
          />
        </PopoverContent>
      </Popover>
    </Field>
  );
}

export default PreviewDateQuestion;
