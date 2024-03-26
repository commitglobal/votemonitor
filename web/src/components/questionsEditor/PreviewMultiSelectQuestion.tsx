import { BaseAnswer, MultiSelectAnswer, MultiSelectQuestion, AnswerType, MultiSelectAnswerSchema } from '@/common/types'
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Button } from '../ui/button';
import { useTranslation } from 'react-i18next';
import { useMemo } from 'react';

import { } from '@/common/types'
import { Checkbox } from '../ui/checkbox';

export interface PreviewMultiSelectQuestionProps {
    languageCode: string;
    question: MultiSelectQuestion;
    answer: MultiSelectAnswer;
    isFirstQuestion: boolean;
    isLastQuestion: boolean;
    onSubmitAnswer: (answer: BaseAnswer) => void;
    onBackButtonClicked: () => void;
}

function PreviewMultiSelectQuestion({
    languageCode,
    question,
    answer,
    isFirstQuestion,
    isLastQuestion,
    onSubmitAnswer,
    onBackButtonClicked }: PreviewMultiSelectQuestionProps) {

    const { t } = useTranslation();

    const form = useForm<MultiSelectAnswer>({
        resolver: zodResolver(MultiSelectAnswerSchema),
        defaultValues: answer ?? {
            questionId: question.id,
            $answerType: AnswerType.MultiSelectAnswerType
        }
    });

    const freeTextOptions = useMemo(() => {
        const freeTextOptions: { [optionId: string]: string } = {};
        question.options.filter(o => o.isFreeText).forEach(o => {
            freeTextOptions[o.id] = o.id;
        });

        return freeTextOptions;
    }, [question]);
    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmitAnswer)} className="space-y-8">
                <FormField
                    control={form.control}
                    name="selection"
                    render={() => (
                        <FormItem>
                            <div className="mb-4">
                                <FormLabel className="text-base">Sidebar</FormLabel>
                            </div>
                            {question.options.map((option) => (
                                <FormField
                                    key={option.id}
                                    control={form.control}
                                    name="selection"
                                    render={({ field }) => {
                                        return (
                                            <FormItem
                                                key={option.id}
                                                className="flex flex-row items-start space-x-3 space-y-0"
                                            >
                                                <FormControl>
                                                    <Checkbox
                                                        checked={field.value?.some(x => x.optionId === option.id)}
                                                        onCheckedChange={(checked: boolean) => {
                                                            return checked
                                                                ? field.onChange([...field.value])
                                                                : field.onChange(
                                                                    field.value?.filter(
                                                                        (value) => value.optionId !== option.id
                                                                    )
                                                                )
                                                        }}
                                                    />
                                                </FormControl>
                                                <FormLabel className="font-normal">
                                                    {option.text[languageCode]}
                                                </FormLabel>
                                            </FormItem>
                                        )
                                    }}
                                />
                            ))}
                            <FormMessage />
                        </FormItem>
                    )}
                />
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
        </Form>
    )
}

export default PreviewMultiSelectQuestion
