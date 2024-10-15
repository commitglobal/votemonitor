import { AnswerType, DateAnswer } from '@/common/types';
import { Label } from '@/components/ui/label';
import { cn } from '@/lib/utils';
import { format, formatISO } from 'date-fns';
import { CalendarIcon } from 'lucide-react';
import { Button } from '../../ui/button';
import { Calendar } from '../../ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '../../ui/popover';
import { useFormAnswersStore } from '../answers-store';

export interface PreviewDateQuestionProps {
  questionId: string;
  text?: string;
  helptext?: string;
  code: string;
}

function PreviewDateQuestion({ code, questionId, text, helptext }: PreviewDateQuestionProps) {
  const { setAnswer, getAnswer } = useFormAnswersStore();
  const answer = getAnswer(questionId) as DateAnswer;

  return (
    <div className="grid gap-6">
      <div className="grid gap-2">
        <Label htmlFor={`${questionId}-value`} className='font-semibold break-all'>{code + ' - '}{text}</Label>
        <Label htmlFor={`${questionId}-value`} className='text-sm italic break-all'>{helptext}</Label>

        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant={'outline'}
              type='button'
              className={cn('w-full pl-3 text-left font-normal', !answer?.date && 'text-muted-foreground')}>
              {answer?.date ? format(answer?.date, 'PPP') : <span>Pick a date</span>}
              <CalendarIcon className='w-4 h-4 ml-auto opacity-50' />
            </Button>
          </PopoverTrigger>
          <PopoverContent className='w-auto p-0' align='start'>
            <Calendar
              mode='single'
              selected={answer?.date ? new Date(answer?.date) : undefined}
              onSelect={(date) => {
                const dateAnswer: DateAnswer = { $answerType: AnswerType.DateAnswerType, date: formatISO(date!, { representation: 'complete' }), questionId };
                setAnswer(dateAnswer);
              }}
              autoFocus
            />
          </PopoverContent>
        </Popover>
      </div>
    </div>

  );
}

export default PreviewDateQuestion;
