import { AnswerType, BaseAnswer, RatingAnswer, RatingQuestion, RatingScaleType } from '@/common/types'
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { Button } from '../../ui/button';
import { zodResolver } from '@hookform/resolvers/zod';
import { RatingGroup } from '../../ui/ratings';
import { z } from 'zod';

export interface PreviewRatingQuestionProps {
    languageCode: string;
    question: RatingQuestion;
    answer: RatingAnswer;
    isFirstQuestion: boolean;
    isLastQuestion: boolean;
    onSubmitAnswer: (answer: BaseAnswer) => void;
    onBackButtonClicked: () => void;
}

function scaleToNumber(scale: RatingScaleType): number {
    switch (scale) {
        case RatingScaleType.OneTo3: return 3;
        case RatingScaleType.OneTo4: return 4;
        case RatingScaleType.OneTo5: return 5;
        case RatingScaleType.OneTo6: return 6;
        case RatingScaleType.OneTo7: return 7;
        case RatingScaleType.OneTo8: return 8;
        case RatingScaleType.OneTo9: return 9;
        case RatingScaleType.OneTo10: return 10;
        default:
            return 5;
    }
}

function PreviewRatingQuestion({ languageCode,
    question,
    answer,
    isFirstQuestion,
    isLastQuestion,
    onSubmitAnswer,
    onBackButtonClicked }: PreviewRatingQuestionProps) {
    const { t } = useTranslation();

    const FormSchema = z.object({
        value: z.string().optional()
    });

    const form = useForm<z.infer<typeof FormSchema>>({
        resolver: zodResolver(FormSchema),
        defaultValues: {
            value: answer?.value?.toString() ?? ''
        }
    });

    function handleSubmit(data: z.infer<typeof FormSchema>) {
        const ratingAnswer: RatingAnswer = {
            questionId: question.id,
            $answerType: AnswerType.RatingAnswerType,
            value: Number(data.value)
        };

        onSubmitAnswer(ratingAnswer);
    }

    return (<Form {...form}>
        <form onSubmit={form.handleSubmit(handleSubmit)}>
            <div className='grid gap-6 py-4 sm:grid-cols-2'>
                <FormField
                    control={form.control}
                    name='value'
                    render={({ field }) => (
                        <FormItem className='flex flex-col'>
                            <FormLabel>{question.text[languageCode]}</FormLabel>
                            {!!question.helptext && <FormDescription>{question.helptext[languageCode]}</FormDescription>}
                            <FormControl>
                                <RatingGroup
                                    scale={scaleToNumber(question.scale)}
                                    {...field}
                                    name='value'
                                    onValueChange={(v) => field.onChange(v)} />
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
                        onClick={() => { onBackButtonClicked(); }}>
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


export default PreviewRatingQuestion
