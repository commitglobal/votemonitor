import { AnswerType, BaseAnswer, NumberAnswer, NumberAnswerSchema, NumberQuestion } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../ui/input';
import { Button } from '../ui/button';
import { useTranslation } from 'react-i18next';

export interface PreviewNumberQuestionProps {
  languageCode: string;
  question: NumberQuestion;
  answer: NumberAnswer;
  isFirstQuestion: boolean;
  isLastQuestion: boolean;
  onSubmitAnswer: (answer: BaseAnswer) => void;
  onBackButtonClicked: () => void;
}

function PreviewNumberQuestion({ 
  languageCode,
  question,
  answer,
  isFirstQuestion,
  isLastQuestion,
  onSubmitAnswer,
  onBackButtonClicked }: PreviewNumberQuestionProps) {
  const { t } = useTranslation();

  const form = useForm<NumberAnswer>({
    resolver: zodResolver(NumberAnswerSchema),
    defaultValues: answer ?? {
      questionId: question.id,
      $answerType: AnswerType.NumberAnswerType,
    }
  });

  return (<Form {...form}>
    <form onSubmit={form.handleSubmit(onSubmitAnswer)}>
      <div className='grid gap-6 py-4 sm:grid-cols-2'>
        <FormField
          control={form.control}
          name='value'
          render={({ field }) => (
            <FormItem className='sm:col-span-2'>
              <FormLabel>{question.text[languageCode]}</FormLabel>
              <FormControl>
                <Input type='number' placeholder={question.inputPlaceholder ? question.inputPlaceholder[languageCode] : ''} {...field} />
              </FormControl>
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

export default PreviewNumberQuestion
