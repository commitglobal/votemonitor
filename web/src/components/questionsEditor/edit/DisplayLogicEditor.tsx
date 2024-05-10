import {
    BaseQuestion,
    DisplayLogic,
    DisplayLogicCondition,
    MultiSelectQuestion,
    QuestionType,
    RatingQuestion,
    SingleSelectQuestion,
} from '@/common/types';
import { Collapsible, CollapsibleContent } from '@/components/ui/collapsible';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Switch } from '@/components/ui/switch';
import { ratingScaleToNumber } from '@/lib/utils';
import { useMemo, useState } from 'react';

interface DisplayLogicEditorProps {
    formQuestions: BaseQuestion[];
    questionIndex: number;
    question: BaseQuestion;
    languageCode: string;
    updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}

const conditions: {
    [questionType: string]: DisplayLogicCondition[];
} = {
    "multiSelectQuestion": ["Includes"],
    "singleSelectQuestion": ["Includes"],
    "numberQuestion": [
        "Equals",
        "NotEquals",
        "LessThan",
        "LessEqual",
        "GreaterThan",
        "GreaterEqual",
    ],
    "ratingQuestion": [
        "Equals",
        "NotEquals",
        "LessThan",
        "LessEqual",
        "GreaterThan",
        "GreaterEqual",
    ],
};

export default function DisplayLogicEditor({
    formQuestions,
    questionIndex,
    question,
    languageCode,
    updateQuestion,
}: DisplayLogicEditorProps) {
    const [hasDisplayLogic, setHasDisplayLogic] = useState(!!question?.displayLogic);

    const parentQuestion = useMemo(() => {
        return formQuestions.find(q => q.id === question?.displayLogic?.parentQuestionId);
    }, [formQuestions, question?.displayLogic?.parentQuestionId]);

    const availableParentQuestions = useMemo(() => {
        return formQuestions
            ?.slice(0, questionIndex)
            ?.filter(q => q.$questionType === QuestionType.SingleSelectQuestionType
                || q.$questionType === QuestionType.MultiSelectQuestionType
                || q.$questionType === QuestionType.RatingQuestionType
                || q.$questionType === QuestionType.NumberQuestionType
            ) ?? [];
    }, [formQuestions, questionIndex]);

    function handleHasDisplayLogicChanged(value: boolean) {
        updateQuestion(questionIndex, {
            ...question,
            displayLogic: value ? {} : undefined
        });

        setHasDisplayLogic(value);
    }

    function handleParentQuestionSelected(questionId: string) {
        const parentQuestion = availableParentQuestions.find(q => q.id === questionId)!;

        const displayLogic: DisplayLogic = {
            parentQuestionId: questionId
        };

        if (parentQuestion?.$questionType === QuestionType.RatingQuestionType) {
            displayLogic.condition = 'Equals';
            displayLogic.value = '1';
        }
        if (parentQuestion?.$questionType === QuestionType.NumberQuestionType) {
            displayLogic.condition = 'Equals';
            displayLogic.value = '0';
        }

        if (parentQuestion?.$questionType === QuestionType.SingleSelectQuestionType
            || parentQuestion?.$questionType === QuestionType.MultiSelectQuestionType) {

            const optionId = (parentQuestion as SingleSelectQuestion | MultiSelectQuestion)!.options[0]?.id
            displayLogic.condition = 'Includes';
            displayLogic.value = optionId;
        }

        updateQuestion(questionIndex, {
            ...question,
            displayLogic: { ...displayLogic }
        });
    }

    function handleConditionChanged(condition: DisplayLogicCondition) {
        updateQuestion(questionIndex, {
            ...question,
            displayLogic: {
                ...question.displayLogic,
                condition
            }
        });
    }

    function handleValueChanged(value: string) {
        updateQuestion(questionIndex, {
            ...question,
            displayLogic: {
                ...question.displayLogic,
                value
            }
        });
    }

    return (
        <div className="mt-3">
            <div className="flex items-center space-x-2">
                <Switch id="has-displayLogic" onCheckedChange={handleHasDisplayLogicChanged} />
                <Label htmlFor="has-displayLogic">Display logic</Label>
            </div>

            <Collapsible open={hasDisplayLogic}>
                <CollapsibleContent className="justify-left flex flex-col mt-3">
                    <div className="justify-left flex flex-col mt-3">
                        <span>When answer for question:</span>
                        <Select
                            name='question'
                            onValueChange={handleParentQuestionSelected}
                            value={parentQuestion?.id}
                        >
                            <SelectTrigger className="min-w-fit flex-1">
                                <SelectValue placeholder='Select question' className="text-xs lg:text-sm" />
                            </SelectTrigger>
                            <SelectContent side='top'>
                                {availableParentQuestions.map((question) => (
                                    <SelectItem key={question.id} value={question.id} >
                                        {question.code + ' - ' + question.text[languageCode]}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>

                    </div>
                    <div className="justify-left flex flex-col mt-3">
                        {
                            parentQuestion &&
                            <Select
                                value={question?.displayLogic?.condition}
                                onValueChange={handleConditionChanged}>
                                <SelectTrigger className="min-w-fit flex-1">
                                    <SelectValue placeholder="Select condition" className="text-xs lg:text-sm" />
                                </SelectTrigger>
                                <SelectContent>
                                    {conditions[parentQuestion.$questionType.toString()]!
                                        .map((condition) =>
                                            <SelectItem
                                                key={condition}
                                                value={condition}
                                                title={condition}
                                                className="text-xs lg:text-sm">
                                                {condition}
                                            </SelectItem>
                                        )}
                                </SelectContent>
                            </Select>
                        }

                    </div>
                    {parentQuestion?.$questionType === QuestionType.NumberQuestionType &&
                        <div className="justify-left flex flex-col mt-3">
                            <Input
                                type='number'
                                value={question?.displayLogic?.value}
                                onChange={(e) => {
                                    handleValueChanged(e.target.value);
                                }}
                                placeholder='value'
                            />
                        </div>
                    }
                    {parentQuestion?.$questionType === QuestionType.RatingQuestionType &&
                        <div className="justify-left flex flex-col mt-3">
                            <span>Value:</span>
                            <Select value={question?.displayLogic?.value} onValueChange={handleValueChanged}>
                                <SelectTrigger className="min-w-fit flex-1">
                                    <SelectValue placeholder="Select rating" className="text-xs lg:text-sm" />
                                </SelectTrigger>
                                <SelectContent>
                                    {[...Array(ratingScaleToNumber((parentQuestion as RatingQuestion)!.scale)).keys()]
                                        .map(value => value + 1)
                                        .map((value) =>
                                            <SelectItem
                                                key={value}
                                                value={value.toString()}
                                                title={value.toString()}
                                                className="text-xs lg:text-sm">
                                                {value}
                                            </SelectItem>
                                        )}
                                </SelectContent>
                            </Select>
                        </div>
                    }
                    {(parentQuestion?.$questionType === QuestionType.MultiSelectQuestionType ||
                        parentQuestion?.$questionType === QuestionType.SingleSelectQuestionType) &&
                        <div className="justify-left flex flex-col mt-3">
                            <Select value={question?.displayLogic?.value} onValueChange={handleValueChanged}>
                                <SelectTrigger className=" min-w-fit flex-1">
                                    <SelectValue placeholder="Select option" className="text-xs lg:text-sm" />
                                </SelectTrigger>
                                <SelectContent>
                                    {(parentQuestion as SingleSelectQuestion | MultiSelectQuestion)!.options
                                        .map((option) =>
                                            <SelectItem
                                                key={option.id}
                                                value={option.id}
                                                title={option.text[languageCode]}
                                                className="text-xs lg:text-sm">
                                                {option.text[languageCode]}
                                            </SelectItem>
                                        )}
                                </SelectContent>
                            </Select>
                        </div>
                    }
                </CollapsibleContent>
            </Collapsible>
        </div>
    );
}
