import { BaseAnswer, MultiSelectAnswer, MultiSelectQuestion, AnswerType } from '@/common/types'
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useMemo, useState } from 'react';

import { } from '@/common/types'
import { z } from 'zod';
import { Checkbox, CheckboxField, CheckboxGroup } from '@/components/ui/checkbox';
import { Field, Fieldset, Label, Legend } from '@/components/ui/fieldset';
import { Text } from '@/components/ui/text';
import { Textarea } from '@/components/ui/textarea';

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
    const [selectedOptionIds, setSelectedOptionIds] = useState<Set<string>>(new Set());

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


    const handleOptionSelected = (optionId: string) => {
        // Since we cannot mutate the state value directly better to instantiate new state with the values of the state
        const optionIds = new Set(selectedOptionIds);

        if (optionIds.has(optionId)) {
            optionIds.delete(optionId);
        } else {
            optionIds.add(optionId);
        }
        setSelectedOptionIds(optionIds);
    };

    return (
        <Fieldset>
            <Legend>{question.code} - {question.text[languageCode]}</Legend>
            {!!question.helptext && <Text>{question.helptext[languageCode]}</Text>}
            <CheckboxGroup>
                {regularOptions.map(option => <CheckboxField key={option.id}>
                    <Checkbox name={option.id} onChange={() => handleOptionSelected(option.id)} />
                    <Label>{option.text[languageCode]}</Label>
                </CheckboxField>)}

                {!!freeTextOption && <CheckboxField>
                    <Checkbox name={freeTextOption.id} onChange={(checked) => {
                        handleOptionSelected(freeTextOption.id);
                        setFreeTextSelected(checked);
                    }} />
                    <Label>{freeTextOption.text[languageCode]}.</Label>
                </CheckboxField>}
            </CheckboxGroup>
            {freeTextSelected && <Field><Textarea /></Field>}
        </Fieldset>
    )
}

export default PreviewMultiSelectQuestion
