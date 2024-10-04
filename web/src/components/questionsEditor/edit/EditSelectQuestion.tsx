import { BaseQuestion, MultiSelectQuestion, QuestionType, SelectOption, SingleSelectQuestion, newTranslatedString } from '@/common/types';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { cn } from '@/lib/utils';
import { PlusIcon, TrashIcon, FlagIcon } from '@heroicons/react/24/solid';
import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { v4 as uuidv4 } from 'uuid';
import QuestionHeader from './QuestionHeader';

export interface EditMultiSelectQuestionProps {
    availableLanguages: string[],
    languageCode: string;
    questionIdx: number;
    isInValid: boolean;
    question: MultiSelectQuestion | SingleSelectQuestion;
    updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}

function EditSelectQuestion({
    availableLanguages,
    languageCode,
    questionIdx,
    isInValid,
    question,
    updateQuestion }: EditMultiSelectQuestionProps) {

    const [invalidOptionId, setInvalidOptionId] = useState<string | null>(null);
    const lastOptionRef = useRef<HTMLInputElement>(null);
    const [freeTextOptionId, setFreeTextOptionId] = useState<string | null>(null);
    const { t } = useTranslation();

    function addOption(optionIdx?: number) {
        let newOptions = !question.options ? [] : question.options;
        const freeTextOption = newOptions.find((option) => option.isFreeText);
        if (freeTextOption) {
            newOptions = newOptions.filter((option) => !option.isFreeText);
        }

        const newOption: SelectOption = {
            id: uuidv4(),
            text: newTranslatedString(availableLanguages, languageCode),
            isFlagged: false,
            isFreeText: false
        };
        if (optionIdx !== undefined) {
            newOptions.splice(optionIdx + 1, 0, newOption);
        } else {
            newOptions.push(newOption);
        }
        if (freeTextOption) {
            newOptions.push(freeTextOption);
        }

        const updatedSelectQuestion: MultiSelectQuestion | SingleSelectQuestion = { ...question, options: newOptions };

        updateQuestion(questionIdx, updatedSelectQuestion);
    };

    function changeQuestionType(newQuestionType: QuestionType.MultiSelectQuestionType | QuestionType.SingleSelectQuestionType) {
        const updatedSelectQuestion: MultiSelectQuestion | SingleSelectQuestion = { ...question, $questionType: newQuestionType };

        updateQuestion(questionIdx, updatedSelectQuestion);
    }

    function addFreeTextOption() {
        if (question.options.filter((option) => option.isFreeText).length === 0) {
            const newOptions = !question.options ? [] : question.options.filter((option) => !option.isFreeText);
            const freeTextOption: SelectOption = {
                id: uuidv4(),
                text: newTranslatedString(availableLanguages, languageCode),
                isFlagged: false,
                isFreeText: true
            };
            newOptions.push(freeTextOption);

            const updatedSelectQuestion: MultiSelectQuestion | SingleSelectQuestion = { ...question, options: newOptions };

            updateQuestion(questionIdx, updatedSelectQuestion);
            setFreeTextOptionId(freeTextOption.id);
        }
    };

    function deleteOption(optionId: string) {
        const newOptions = !question.options ? [] : question.options.filter((option) => option.id !== optionId);

        if (invalidOptionId === optionId) {
            setInvalidOptionId(null);
        }

        const updatedSelectQuestion: MultiSelectQuestion | SingleSelectQuestion = { ...question, options: newOptions };
        updateQuestion(questionIdx, updatedSelectQuestion);
        setFreeTextOptionId(optionId === freeTextOptionId ? null : freeTextOptionId);
    };

    function updateOption(optionId: string, text: string) {
        // debugger;
        const newOptions = question.options.map((option) => {
            if (option.id === optionId) {
                const newText = option.text;
                newText[languageCode] = text;

                return {
                    ...option,
                    text: newText
                }
            }

            return { ...option };
        });

        const updatedSelectQuestion: MultiSelectQuestion | SingleSelectQuestion = { ...question, options: newOptions };
        updateQuestion(questionIdx, updatedSelectQuestion);
    }

    function updateOptionFlag(optionId: string) {
        // debugger;
        const newOptions = question.options.map((option) => {
            if (option.id === optionId) {
                const isFlagged = option.isFlagged;

                return {
                    ...option,
                    isFlagged: !isFlagged
                }
            }

            return { ...option };
        });

        const updatedSelectQuestion: MultiSelectQuestion | SingleSelectQuestion = { ...question, options: newOptions };
        updateQuestion(questionIdx, updatedSelectQuestion);
    }

    function getOptionIdWithDuplicateLabel(): string | null {
        for (let i = 0; i < question.options.length; i++) {
            for (let j = i + 1; j < question.options.length; j++) {
                if (question.options[i]!.text[languageCode]!.trim() === question.options[j]!.text[languageCode]!.trim()) {
                    return question.options[i]!.id;
                }
            }
        }
        return null;
    };

    function getOptionIdWithEmptyLabel(): string | null {
        // debugger;
        for (let i = 0; i < question.options.length; i++) {
            if (question.options[i]!.text[languageCode]!.trim() === "") return question.options[i]!.id;
        }
        return null;
    };


    useEffect(() => {
        if (lastOptionRef.current) {
            lastOptionRef.current?.focus();
        }
    }, [question.options?.length]);

    return (
        <div>
            <QuestionHeader
                availableLanguages={availableLanguages}
                languageCode={languageCode}
                isInValid={isInValid}
                question={question}
                questionIdx={questionIdx}
                updateQuestion={updateQuestion}
            />

            <div className="mt-3">
                <Label htmlFor="choices">Options</Label>
                <div className="mt-2 space-y-2" id="choices">
                    {question.options &&
                        question.options.map((option, optionIdx) => (
                            <div key={optionIdx} className="inline-flex w-full items-center">
                                <Input
                                    ref={optionIdx === question.options.length - 1 ? lastOptionRef : null}
                                    id={option.id}
                                    name={option.id}
                                    value={option.text[languageCode]}
                                    className={cn(option.id === freeTextOptionId && "border-dashed", ((invalidOptionId === "" && option.text[languageCode]!.trim() === "") ||
                                        (invalidOptionId !== null && option.text[languageCode]!.trim() === invalidOptionId.trim())) && "border-red-300 focus:border-red-300")}
                                    placeholder={`Option ${optionIdx + 1}`}
                                    onBlur={() => {
                                        const optionIdWithDuplicatedLabel = getOptionIdWithDuplicateLabel();
                                        if (optionIdWithDuplicatedLabel) {
                                            setInvalidOptionId(optionIdWithDuplicatedLabel);
                                        } else {
                                            const optionIdWithEmptyLabel = getOptionIdWithEmptyLabel()
                                            if (optionIdWithEmptyLabel) {
                                                setInvalidOptionId(optionIdWithEmptyLabel);
                                            } else {
                                                setInvalidOptionId(null);
                                            }
                                        }
                                    }}
                                    onChange={(e) => updateOption(option.id, e.target.value)}
                                />
                                {question.options && question.options.length > 2 && (
                                    <TrashIcon
                                        className="ml-2 h-4 w-4 cursor-pointer text-slate-400 hover:text-slate-500"
                                        onClick={() => deleteOption(option.id)}
                                    />
                                )}
                                <FlagIcon
                                    className="ml-2 h-4 w-4 cursor-pointer text-slate-400 hover:text-slate-500"
                                    onClick={() => updateOptionFlag(option.id)}
                                />

                                <div className="ml-2 h-4 w-4">
                                    {option.id !== freeTextOptionId && (
                                        <PlusIcon
                                            className="h-full w-full cursor-pointer text-slate-400 hover:text-slate-500"
                                            onClick={() => addOption(questionIdx)}
                                        />
                                    )}
                                </div>
                            </div>
                        ))}
                    <div className="flex items-center justify-between space-x-2">
                        {question.options.filter((c) => c.isFreeText).length === 0 && (
                            <Button size="sm" variant="outline" type="button" onClick={() => addFreeTextOption()}>
                                {t('questionEditor.selectQuestion.addFreeTextOption')}
                            </Button>
                        )}
                        <Button
                            size="sm"
                            variant="outline"
                            type="button"
                            onClick={() => {
                                question.$questionType === QuestionType.SingleSelectQuestionType
                                    ? changeQuestionType(QuestionType.MultiSelectQuestionType)
                                    : changeQuestionType(QuestionType.SingleSelectQuestionType);
                            }}>
                            {
                                question.$questionType === QuestionType.SingleSelectQuestionType
                                    ? t('questionEditor.selectQuestion.toMultiSelectQuestion')
                                    : t('questionEditor.selectQuestion.toSingleSelectQuestion')
                            }
                        </Button>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default EditSelectQuestion
