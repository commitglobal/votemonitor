import { BaseAnswer, MultiSelectAnswer, MultiSelectQuestion, AnswerType } from '@/common/types'
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '../../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useMemo, useState } from 'react';

import { } from '@/common/types'
import { Checkbox } from '../../ui/checkbox';
import { Button } from '../../ui/button';
import { Input } from '../../ui/input';
import { z } from 'zod';

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

    const FormSchema = z.object({
        selectedOptions: z.array(z.string()).optional(),
        freeText: z.string().optional()
    });

    const form = useForm<z.infer<typeof FormSchema>>({
        resolver: zodResolver(FormSchema),
        defaultValues: {
            selectedOptions: answer?.selection?.map(s => s.optionId) ?? [],
            freeText: answer?.selection?.map(s => s.text).filter(t => !!t)[0] ?? ''
        }
    });

    const [freeTextSelected, setFreeTextSelected] = useState(false);

    const regularOptions = useMemo(() => {
        if (!question.options) {
            return [];
        }
        const regularOptions = question.options.filter((option) => !option.isFreeText);
        return regularOptions;
    }, [question.options]);

    // Currently we only support one free text option
    const freeTextOption = useMemo(
        () => question.options.find((option) => option.isFreeText),
        [question.options]
    );

    function handleSubmit(data: z.infer<typeof FormSchema>) {
        const selection = data?.selectedOptions?.map(option => {
            if (option === freeTextOption?.id) {
                return { optionId: option, text: data.freeText }
            }
            return { optionId: option }
        });

        const multiSelectAnswer: MultiSelectAnswer = {
            questionId: question.id,
            $answerType: AnswerType.MultiSelectAnswerType,
            selection: selection ?? []
        };

        onSubmitAnswer(multiSelectAnswer);
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-8">
                <FormField
                    control={form.control}
                    name="selectedOptions"
                    render={() => (
                        <FormItem>
                            <div className="mb-4">
                                <FormLabel className="text-base">{question.text[languageCode]}</FormLabel>
                                {!!question.helptext && <FormDescription>
                                    {question.helptext[languageCode]}
                                </FormDescription>
                                }
                            </div>
                            {regularOptions.map((option) => (
                                <FormField
                                    key={option.id}
                                    control={form.control}
                                    name="selectedOptions"
                                    render={({ field }) => {
                                        return (
                                            <FormItem
                                                key={option.id}
                                                className="flex flex-row items-start space-x-3 space-y-0"
                                            >
                                                <FormControl>
                                                    <Checkbox
                                                        checked={field.value?.includes(option.id)}
                                                        onCheckedChange={(checked) => {
                                                            return checked
                                                                ? field.onChange([...field.value!, option.id])
                                                                : field.onChange(
                                                                    field.value?.filter(
                                                                        (value) => value !== option.id
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

                            {!!freeTextOption &&
                                <FormField
                                    key={freeTextOption.id}
                                    control={form.control}
                                    name="selectedOptions"
                                    render={({ field }) => {
                                        return (
                                            <FormItem
                                                key={freeTextOption.id}
                                                className="flex flex-row items-start space-x-3 space-y-0"
                                            >
                                                <FormControl>
                                                    <Checkbox
                                                        checked={field.value?.includes(freeTextOption.id)}
                                                        onCheckedChange={(checked) => {
                                                            if (checked) {
                                                                setFreeTextSelected(true);
                                                                field.onChange([...field.value!, freeTextOption.id]);
                                                            }
                                                            else {
                                                                setFreeTextSelected(false);
                                                                field.onChange(
                                                                    field.value?.filter(
                                                                        (value) => value !== freeTextOption.id
                                                                    )
                                                                )
                                                            }
                                                        }}
                                                    />
                                                </FormControl>
                                                <FormLabel className="font-normal">
                                                    {freeTextOption.text[languageCode]}
                                                </FormLabel>
                                            </FormItem>
                                        )
                                    }}
                                />}

                            {freeTextSelected &&
                                <FormField
                                    control={form.control}
                                    name="freeText"
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormControl>
                                                <Input {...field} placeholder={t("app.input.pleaseSpecify")} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />}
                            <FormMessage />
                        </FormItem>
                    )}
                />
            </form>
        </Form>
    )
}

export default PreviewMultiSelectQuestion
