import { AnswerType, BaseAnswer, NumberAnswer, TextAnswerSchema, NumberQuestion, TextAnswer, TextQuestion } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Input } from '../../ui/input';
import { Button } from '../../ui/button';
import { useTranslation } from 'react-i18next';

export interface PreviewTextQuestionProps {
    languageCode: string;
    question: TextQuestion;
    answer: TextAnswer;
    isFirstQuestion: boolean;
    isLastQuestion: boolean;
    onSubmitAnswer: (answer: BaseAnswer) => void;
    onBackButtonClicked: () => void;
}

function PreviewTextQuestion({ 
    languageCode,
    question,
    answer,
    isFirstQuestion,
    isLastQuestion,
    onSubmitAnswer,
    onBackButtonClicked }: PreviewTextQuestionProps) {
    const { t } = useTranslation();

    const form = useForm<TextAnswer>({
        resolver: zodResolver(TextAnswerSchema),
        defaultValues: answer ?? {
            questionId: question.id,
            $answerType: AnswerType.TextAnswerType,
        }
    });

    return (<Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmitAnswer)}>
            <div className='grid gap-6 py-4 sm:grid-cols-2'>
                <FormField
                    control={form.control}
                    name='text'
                    render={({ field }) => (
                        <FormItem className='sm:col-span-2'>
                            <FormLabel>{question.text[languageCode]}</FormLabel>
                            <FormControl>
                                <Input placeholder={question.inputPlaceholder ? question.inputPlaceholder[languageCode] : ''} {...field} />
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

export default PreviewTextQuestion
